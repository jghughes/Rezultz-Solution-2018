using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;

#region IMPORTANT NOTE ABOUT AYNC METHODS AND .CONFIGUREAWAIT

/*  
    * Everywhere on the UI thread, it is essential for .ConfigureAwait to be True for all async method calls. 
    * True happens to be the system default for async methods, so safe practice is to remain silent about the setting.
    * Everywhere other than on the UI thread, Microsoft's cautiously, hesitantly recommended default 
    * is False for reasons which are amply ventilated on MSDN to do with the marshalling of threads. This is problemtic.
    * The great difficulty emerges inside classes or class libraries where the author of a method has no way
    * of knowing in advance if the method is going to be marshalled back to the UI thread or not.
    * Generally speaking your initial best bet is experiment with True to preempt a "cross-thread access" exception.
    * In some settings there is a problem caused by context and the location of the offending method with this approach. 
    * For example, merely refactoring a hierarchy of calls, whilst making no changes whatsover to the constituent tasks 
    * in the hierarchy, can cause problems to materialise out of nowhere. Sometimes a method will work 
    * perfectly with a given switch in one location, but the opposite in another. The only recourse 
    * is to empirical testing. Unfortunately, you have no option but to use False in certain circumstances 
    * otherwise your async method will hang inexplicably. There is no way to predict. You have to test.
    */

#endregion

namespace NetStd.Goodies.Mar2022
{
    /// <summary>
    ///     Parallel and concurrent processing utilities.
    ///     Use parallel for CPU-intensive operations. Work-stealing algorithm is much faster than Partitioning.
    ///     Use WhenAll for concurrency in I/O intensive operations. 
    /// </summary>
    public static class JghParallel
    {
        private const string Locus2 = nameof(JghParallel);
        private const string Locus3 = "[NetStd.Goodies.Mar2022]";

        #region methods for concurrent I/O

        /// <summary>
        ///     Performs collection of tasks concurrently.
        ///     Null tasks are skipped.
        ///     Intended for I/O operations (not computational tasks).
        ///     Try/Catch format ensures that the exception thrown (if any)
        ///     is an Aggregate exception that includes all constituent exceptions,
        ///     not just the first exception that happened to be thrown.
        /// </summary>
        /// <typeparam name="T">Common type of returned object</typeparam>
        /// <param name="tasks">Collection of tasks with same type of return value</param>
        /// <returns>Collection of objects returned upon completion of each task</returns>
        public static async Task<T[]> WhenAllAsync<T>(Task<T>[] tasks) where T : class
        {
            if (tasks == null)
                return [];

            var nonNullTasks = tasks
                .Where(task => task != null).ToList();

            var allTasks = Task.WhenAll(nonNullTasks);

            try
            {
                T[] answer = await allTasks;

                return answer;
            }
            catch (Exception)
            {
                //ignore

            }


            throw allTasks.Exception ?? throw new Exception("Dummy. can never reach this statement if Exception is null");
        }

        /// <summary>
        ///     Performs collection of void tasks concurrently.
        ///     Null tasks are skipped.
        ///     Intended for I/O operations (not computational tasks).
        ///     Try/Catch format ensures that the exception thrown (if any)
        ///     is an Aggregate exception that includes all constituent exceptions,
        ///     not just the first exception that happened to be thrown.
        /// </summary>
        /// <param name="tasks">Collection of tasks with void return value</param>
        /// <returns>void</returns>
        public static async Task WhenAllAsync<T>(Task[] tasks)
        {
            if (tasks == null)
                return;

            var nonNullTasks = tasks
                .Where(task => task != null).ToList();

            var allTasks = Task.WhenAll(nonNullTasks);

            try
            {
                await allTasks;

                return;
            }
            catch (Exception)
            {
                //ignore

            }

            throw allTasks.Exception ?? throw new Exception("Dummy. can never reach this statement if Exception is null");
        }

        #endregion

        #region methods for parallel CPU-intensive ops to select/project/transform a collection

        /// <summary>
        ///     Projects each element of a sequence into a new form.
        ///     Source is partitioned according to the number of logical processors.
        ///     Each partition is processed on a standalone thread.
        ///     Order is preserved.
        ///     Performance deteriorates with small sizes of collections and when workload fopr each collection item is trivial.
        /// </summary>
        /// <typeparam name="TSource">Element type</typeparam>
        /// <typeparam name="TResult">New form of type</typeparam>
        /// <param name="source">Generic type of collection</param>
        /// <param name="transform">Transform function</param>
        /// <param name="itemCountThresholdForParallelisation">Collection smaller than size threshold is not parallelised</param>
        /// <returns>Empty if source is null or empty. Unaltered if transform is null.</returns>
        public static async Task<TResult[]> SelectAsParallelPartitionedAsync<TSource, TResult>(IList<TSource> source, Func<TSource, TResult> transform, int itemCountThresholdForParallelisation)
            where TSource : class
            where TResult : class
        {
            const string failure = "Unable to select and project collection of items.";
            const string locus = "[SelectAsParallelPartitionedAsync]";

            #region check for null parameters

            if (source == null) return [];

            if (!source.Any()) return [];

            if (transform == null) return [];

            #endregion

            #region  if small dataset take the fast track - do it sequentially and exit

            if (source.Count < System.Math.Max(1, itemCountThresholdForParallelisation))
                return source.Select(transform).ToArray();

            //if (source.Count < JghMath.Max(1, itemCountThresholdForParallelisation))
            //    return source.Select(transform).ToArray();

            #endregion

            #region large dataset justifying parallelisation to a greater or lesser degree. create a keyed collection of tasks to execute in parallel. key is used to preserve order

            var maxDegreeofParallelisation = Environment.ProcessorCount;

            var partitions =
                MakeKeyedPartition(source, itemCountThresholdForParallelisation, maxDegreeofParallelisation);

            var arrayOfTaskOutputs = new KeyValuePair<int, TResult[]>[partitions.Count()];

            var taskCollection = (from partitionKvp in partitions
                select SelectArrayOnAStandAloneThreadAsync(partitionKvp, transform)).ToArray();

            #endregion

            try
            {
                #region do the work

                var allTasksInContainers = MakeIntoInterleavedCollectionOfPackagedTasksAsync(taskCollection);

                foreach (var taskContainer in allTasksInContainers)
                {
                    var task = await taskContainer;

                    if (task.IsFaulted)
                        throw task.Exception ??
                              new Exception("Task ended in a faulted state. Additional information unavailable.");

                    var resultAsKvp = task.Result;

                    if (resultAsKvp.Value != null)
                        arrayOfTaskOutputs[resultAsKvp.Key] = resultAsKvp;
                }

                var answer = UndoPartition(arrayOfTaskOutputs.ToArray());

                #endregion

                return answer;
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /* Note:
        * For reasons i know not, the SelectAsParallelWorkStealingAsync library blows up with catalogue items. 
        * my unconfirmed suspicion is that this is because the item type includes a BitmapImage - which is Silverlight specific.
        * var answerAsCustomTypes = await childXElements.SelectAsParallelWorkStealingAsync(CustomTypeXmlParsingAndSerialisationHelpers.MapFreeFormXElementToCatalogueItem, 30).ConfigureAwait(false);
        */
        /// <summary>
        ///     Projects each element of a sequence into a new form.
        ///     Source is processed a single record at a time.
        ///     Work is stolen and processed by multiple threads, according to the number of logical processors.
        ///     Order is preserved.
        ///     Performance compares favourably to PLinq.
        /// </summary>
        /// <typeparam name="TSource">Element type</typeparam>
        /// <typeparam name="TResult">New type of form</typeparam>
        /// <param name="source">Generic type of collection</param>
        /// <param name="transform">Transform function</param>
        /// <param name="itemCountThresholdForParallelisation">Collection smaller than size threshold is not parallelised</param>
        /// <returns>Empty if source is null or empty. Unaltered if transform is null.</returns>
        public static async Task<TResult[]> SelectAsParallelWorkStealingAsync<TSource, TResult>(
            IList<TSource> source,
            Func<TSource, TResult> transform,
            int itemCountThresholdForParallelisation)
            where TSource : class
            where TResult : class
        {
            const string failure = "Unable to select and project collection of items.";
            const string locus = "[SelectAsParallelWorkStealingAsync]";

            TResult[] answer;

            try
            {
                #region check for null parameters

                if (source == null) return [];

                if (!source.Any()) return [];

                if (transform == null) return [];

                #endregion

                #region  if small dataset take the fast track - do it sequentially and exit

                if (source.Count < System.Math.Max(1, itemCountThresholdForParallelisation))
                    return source.Select(transform).ToArray();
                //if (source.Count < JghMath.Max(1, itemCountThresholdForParallelisation))
                //    return source.Select(transform).ToArray();

                #endregion

                #region  on we go .... set up buckets to receive the result from each thread (or an exception) based on the number of threads i.e. the number of processors

                var threadCount = Environment.ProcessorCount;

                var taskFaults = new Exception[threadCount];

                answer = new TResult[source.Count];

                #endregion

                #region do the parallelised work

                IList<KeyValuePair<int, TSource>> indexedSource = EstablishTheOrder(source);

                if (indexedSource == null) return [];

                var remainingActiveThreads = threadCount;

                using (var enumerator = indexedSource.GetEnumerator())
                {
                    using var myManualResetEvent = new ManualResetEvent(false);
                    // launch the background worker threads

                    for (var i = 0; i < threadCount; i++)
                    {
                        var threadId = i;

                        ThreadPool.QueueUserWorkItem(
                            delegate
                            {
                                var thredId = threadId;

                                    #region delegate for this thread

                                    var possibleCulpritRecordAsString = string.Empty;

                                    // Iterate (collectively in parallel) until there's no more work or when the first exception occurs ..... 

                                    var anExceptionHasOccurredOnThisThreadInsideThisLoop = false;

                                while (!anExceptionHasOccurredOnThisThreadInsideThisLoop)
                                {
                                        // try block to isolate any exception thrown by the transform and to propogate information on the culprit record

                                        try
                                    {
                                        KeyValuePair<int, TSource> nextItem;

                                            // ReSharper disable once AccessToDisposedClosure
                                            lock (enumerator)
                                        {
                                                // ReSharper disable once AccessToDisposedClosure
                                                if (!enumerator.MoveNext()) break;

                                                // ReSharper disable once AccessToDisposedClosure
                                                nextItem = enumerator.Current;
                                        }

                                        possibleCulpritRecordAsString =
                                            nextItem.Value?.ToString() ??
                                            string.Empty; // just in case value is null

                                            var transformedValue = transform(nextItem.Value);

                                        answer[nextItem.Key] = transformedValue;
                                    }

                                        #region trycatch. save exception in the correct bucket and exit the while loop by setting anExceptionHasOccurred to true

                                        catch (Exception ex)
                                    {
                                        anExceptionHasOccurredOnThisThreadInsideThisLoop =
                                            true; // set flag so as to exit the loop and thus this thread next thing

                                            var sb = new StringBuilder();
                                        sb.AppendLine();
                                        sb.AppendLine(
                                            $"Exception occurred on thread number {thredId} of {threadCount}. {ex.Message}");
                                        sb.AppendLine(string.Format(possibleCulpritRecordAsString));
                                        sb.AppendLine("Unable to process the above data item.");

                                        taskFaults[thredId] = new Exception(sb.ToString(), ex);
                                    }

                                        #endregion
                                    }

                                    // decrement the number of remaining busy threads by one
                                    if (Interlocked.Decrement(ref remainingActiveThreads) == 0)
                                        // ReSharper disable once AccessToDisposedClosure
                                        myManualResetEvent.Set();

                                    #endregion
                                });
                    }

                    //Wait for all workers to complete

                    myManualResetEvent.WaitOne();
                }

                #endregion

                #region bale if one or more faults occurred on all threads

                var collectionOfExceptions = taskFaults.Where(z => z != null).ToArray();

                if (collectionOfExceptions.Any())
                    throw new AggregateException(collectionOfExceptions);

                #endregion

                return answer;
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #region helpers

        /// <summary>
        ///     Subdivides source collection into partitions of near equal size subject to a minimum size threshold.
        ///     Order is preserved.
        /// </summary>
        /// <typeparam name="T">Reference type of source</typeparam>
        /// <param name="sourceCollection">Source collection</param>
        /// <param name="sizeThresholdForPartition">Number of items that trigger addition of a partition</param>
        /// <param name="preferredNumberOfPartitions">
        ///     Note: if used for parallelisation, set this to the number of logical
        ///     processors i.e. Environment.ProcessorCount
        /// </param>
        /// <returns>Array of arrays of T</returns>
        private static T[][] MakePartition<T>(IList<T> sourceCollection, int sizeThresholdForPartition, int preferredNumberOfPartitions)
            where T : class
        {
            const string failure = "Unable to subdivide collection of items into partitions.";
            const string locus = "[MakePartition]";

            try
            {
                #region check paramaters

                if (sourceCollection == null) return null;

                if (!sourceCollection.Any()) return [];

                if (sizeThresholdForPartition < 1)
                    sizeThresholdForPartition = 1;

                if (preferredNumberOfPartitions < 1)
                    preferredNumberOfPartitions = 1;

                #endregion

                #region figure out sensible number of records per partition and sensible number of partitions

                var source = sourceCollection.ToArray();

                var sourceCount = source.Count();
                var theoreticalNumberOfThresholdSizedPartitions =
                    Convert.ToInt32(sourceCount / Convert.ToDouble(sizeThresholdForPartition));

                var numberOfPartitions =
                    Math.Min(theoreticalNumberOfThresholdSizedPartitions, preferredNumberOfPartitions);
                //var numberOfPartitions =
                //    JghMath.Min(theoreticalNumberOfThresholdSizedPartitions, preferredNumberOfPartitions);

                var quotaOfItemsForEachPartition = sourceCount / numberOfPartitions;
                var numberOfRemainders = sourceCount - quotaOfItemsForEachPartition * numberOfPartitions;

                var remainders = new Queue<T>();

                #endregion

                #region short cut to the exits if a single partition makes sense

                if (numberOfPartitions < 2)
                {
                    var quickAnswer = new T[1][];

                    quickAnswer[0] = source.ToArray();

                    return quickAnswer;
                }

                #endregion

                #region load up the partitions

                var partitions = new List<List<T>>();

                var i = 0;

                var currentPartition = new List<T>();

                while (i < sourceCount)
                {
                    currentPartition.Add(source[i]);

                    i++;

                    var quotaOfCurrentPartitionIsFull = !(currentPartition.Count < quotaOfItemsForEachPartition);

                    if (quotaOfCurrentPartitionIsFull)
                    {
                        partitions.Add(currentPartition);

                        currentPartition = [];

                        var bucketOfRemaindersIsNotFull = remainders.Count < numberOfRemainders;

                        if (bucketOfRemaindersIsNotFull)
                        {
                            remainders.Enqueue(source[i]);

                            i++;
                        }
                    }
                }

                if (currentPartition.Any())
                    partitions.Add(currentPartition);

                // go back and distribute the remainders correctly

                i = 0;

                while (remainders.Any())
                {
                    partitions[i].Add(remainders.Dequeue());

                    i++;
                }

                #endregion

                var zz = partitions.Select(partition => partition.ToArray());

                var answer = zz.ToArray();

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Subdivides source collection into partitions of near equal size subject to a minimum size threshold.
        ///     Order is preserved. Each partition is keyed with an integer index and can thus be tracked.
        /// </summary>
        /// <typeparam name="T">Reference type of source</typeparam>
        /// <param name="sourceCollection">Source collection</param>
        /// <param name="sizeThresholdForPartition">Number of items that trigger addition of a partition</param>
        /// <param name="preferredNumberOfPartitions">
        ///     Note: if used for parallelisation, set this to the number of logical
        ///     processors i.e. Environment.ProcessorCount
        /// </param>
        /// <returns>Array of key value pairs. Each kvp is an array of T with an associated index</returns>
        private static KeyValuePair<int, T[]>[] MakeKeyedPartition<T>(IList<T> sourceCollection, int sizeThresholdForPartition, int preferredNumberOfPartitions)
            where T : class
        {
            const string failure = "Unable to subdivide collection of items into keyed collection of partitions.";
            const string locus = "[MakeKeyedPartition]";

            try
            {
                var partitions = MakePartition(sourceCollection, sizeThresholdForPartition,
                    preferredNumberOfPartitions);


                var keyedPartitions = new KeyValuePair<int, T[]>[partitions.Count()];

                var i = 0;
                foreach (var partition in partitions)
                {
                    keyedPartitions[i] = new KeyValuePair<int, T[]>(i, partition);
                    i++;
                }
                return keyedPartitions;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Flattens an outer collection of inner collections.
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="source">Outer collection is of key value pairs. Each kvp consists of an index and an inner collection.</param>
        /// <returns>Flattened collection</returns>
        private static T[] UndoPartition<T>(IEnumerable<KeyValuePair<int, T[]>> source) where T : class
        {
            if (source == null) return null;

            var outerCollection = source.ToArray();

            if (!outerCollection.Any()) return [];

            var flattenedAnswer = new List<T>();

            foreach (var innerCollection in outerCollection.OrderBy(z => z.Key).ToArray())
                flattenedAnswer.AddRange(innerCollection.Value.ToList());

            return flattenedAnswer.ToArray();
        }

        /// <summary>
        ///     Flattens an outer collection of inner collections.
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="source">Outer collection and inner collections must be enumerable.</param>
        /// <returns>Flattened collection</returns>
        private static T[] UndoPartition<T>(IEnumerable<T[]> source) where T : class, new()
        {
            if (source == null) return null;

            var outerCollection = source.ToArray();

            if (!outerCollection.Any()) return [];

            var flattenedSource = new List<T>();

            foreach (var innerCollection in outerCollection.Where(z => z != null))
                flattenedSource.AddRange(innerCollection);

            return flattenedSource.ToArray();
        }

        /// <summary>
        ///     Projects each element of a sequence into an indexed key value pair
        /// </summary>
        /// <typeparam name="TSource">Generic type</typeparam>
        /// <param name="source">source collection</param>
        /// <returns>Array of key value pairs</returns>
        private static KeyValuePair<int, TSource>[] EstablishTheOrder<TSource>(IEnumerable<TSource> source)
            where TSource : class
        {
            var zz = new List<KeyValuePair<int, TSource>>();

            var index = 0;

            foreach (var item in source)
            {
                zz.Add(new KeyValuePair<int, TSource>(index, item));
                index++;
            }
            return zz.ToArray();
        }

        private static async Task<TResult[]> SelectOnAStandAloneThreadAsync<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> transform)
            where TSource : class
            where TResult : class
        {
            const string failure = "Unable to select and project collection of items.";
            const string locus = "[SelectOnAStandAloneThreadAsync]";


            if (source == null) return [];

            if (!source.Any()) return [];

            if (transform == null) return [];

            try
            {
                var outcome = Task.Run(
                    () =>
                    {
                        var transformedItem = source.Where(z => z != null).Select(transform);

                        return transformedItem;
                    });

                //var outcome = TaskEx.Run(
                //    () =>
                //    {
                //        var transformedItem = source.Where(z => z != null).Select(transform);

                //        return transformedItem;
                //    });

                await outcome;

                var answer = outcome.Result;

                return answer.ToArray();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private static async Task<KeyValuePair<int, TResult[]>> SelectArrayOnAStandAloneThreadAsync<TSource, TResult>(KeyValuePair<int, TSource[]> item, Func<TSource, TResult> transform)
            where TSource : class
            where TResult : class
        {
            const string failure = "Unable to select and project collection of items.";
            const string locus = "[SelectArrayAsync]";

            //  Note: don't use this as standalone parallel Select method. on its own it confers no speed up

            if (item.Value == null) return new KeyValuePair<int, TResult[]>(item.Key, null);

            if (!item.Value.Any()) return new KeyValuePair<int, TResult[]>(item.Key, []);

            try
            {
                KeyValuePair<int, TResult[]> answer;

                var zz = item.Value.Where(z => z != null).ToArray();

                var transformedArray = await zz.SelectOnAStandAloneThreadAsync(transform);

                answer = new KeyValuePair<int, TResult[]>(item.Key, transformedArray.ToArray());

                return answer;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private static Task<Task<T>>[] MakeIntoInterleavedCollectionOfPackagedTasksAsync<T>(IEnumerable<Task<T>> tasks) //where T : class
        {
            var listOfTasks = tasks.ToList();

            var count = listOfTasks.Count();

            var tcs = new TaskCompletionSource<Task<T>>[count];

            var result = new Task<Task<T>>[count];

            for (var i = 0; i < count; i++)
            {
                tcs[i] = new TaskCompletionSource<Task<T>>();

                result[i] = tcs[i].Task;
            }

            var nextTaskIndex = -1;

            Action<Task<T>> continuation = completed =>
            {
                var taskCompletionSourceBucket = tcs[Interlocked.Increment(ref nextTaskIndex)];

                taskCompletionSourceBucket.TrySetResult(completed);
            };

            // the meat

            foreach (var task in listOfTasks)
                task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

            return result;
        }

        private static IEnumerable<Task<Task>> MakeIntoInterleavedCollectionOfPackagedTasksAsync(IEnumerable<Task> tasks)
        {
            var listOfTasks = tasks.ToList();

            var count = listOfTasks.Count();

            var tcs = new TaskCompletionSource<Task>[count];

            var result = new Task<Task>[count];

            for (var i = 0; i < count; i++)
            {
                tcs[i] = new TaskCompletionSource<Task>();

                result[i] = tcs[i].Task;
            }

            var nextTaskIndex = -1;

            Action<Task> continuation = completed =>
            {
                var taskCompletionSourceBucket = tcs[Interlocked.Increment(ref nextTaskIndex)];

                taskCompletionSourceBucket.TrySetResult(completed);
            };

            // the meat

            foreach (var task in listOfTasks)
                task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

            return result;
        }

        #endregion

        #endregion

        #region async threadsafe queue and stack for producer consumer pattern

        /// <summary>
        ///     Simple threadsafe queue with async Add and TakeAsync.
        ///     For applicability in producer/consumer pattern see page 29 of 38 in white paper entitled
        ///     The Task-based Asynchronous Pattern by Stephen Toub dated Feb 2012
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        public class SafeQueueAsync<T> where T : class
        {
            private readonly object _arbitraryLockingObject = new();
            private readonly Queue<T> _mCollection;

            private readonly Queue<TaskCompletionSource<T>> _mWaiting;

            public SafeQueueAsync()
            {
                lock (_arbitraryLockingObject)
                {
                    _mCollection = new Queue<T>();
                    _mWaiting = new Queue<TaskCompletionSource<T>>();
                }
            }

            public SafeQueueAsync(int capacity)
            {
                lock (_arbitraryLockingObject)
                {
                    _mCollection = new Queue<T>(capacity);
                    _mWaiting = new Queue<TaskCompletionSource<T>>(capacity);
                }
            }

            /// <summary>
            ///     async method
            /// </summary>
            /// <param name="item">generic type</param>
            public void Add(T item)
            {
                TaskCompletionSource<T> tcs = null;

                lock (_arbitraryLockingObject)
                {
                    if (_mWaiting.Count > 0)
                        tcs = _mWaiting.Dequeue();
                    else
                        _mCollection.Enqueue(item);
                }

                tcs?.TrySetResult(item);
            }

            /// <summary>
            /// </summary>
            /// <returns>Task typeof T</returns>
            public Task<T> TakeAsync()
            {
                lock (_arbitraryLockingObject)
                {
                    if (_mCollection.Count > 0)
                    {
                        //// use Task<TResult>.FromResult to create a task that holds a pre-computed result
                        //return Task.FromResult(_mCollection.Dequeue());


                        // Jgh version - works with silverlight as well 
                        //----------------------------------------------------------------
                        var tcs = new TaskCompletionSource<T>();

                        tcs.TrySetResult(_mCollection.Dequeue());
                        return tcs.Task;

                        //----------------------------------------------------------------
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<T>();

                        _mWaiting.Enqueue(tcs);

                        return tcs.Task;
                    }
                }
            }

            public bool Any()
            {
                lock (_arbitraryLockingObject)
                {
                    return _mCollection.Any();
                }
            }

            public int Count()
            {
                lock (_arbitraryLockingObject)
                {
                    return _mCollection.Count;
                }
            }

            public T[] ToArrayAndClear()
            {
                var numItems = Count();

                var answer = new T[numItems];

                lock (_arbitraryLockingObject)
                {
                    for (var i = 0; i < numItems; i++)
                    {
                        var safeIndex = i;

                        answer[safeIndex] = TakeAsync().Result;
                    }

                    return answer;
                }
            }

            public List<T> ToListAndClear()
            {
                var numItems = Count();

                var answer = new List<T>(numItems);

                lock (_arbitraryLockingObject)
                {
                    for (var i = 0; i < numItems; i++)
                        answer.Add(TakeAsync().Result);

                    return answer;
                }
            }

            public string PrintAllElements()
            {
                var output = new StringBuilder();

                //Lock the queue.
                lock (_arbitraryLockingObject)
                {
                    foreach (var elem in _mCollection)
                        // Print the next element.
                        output.AppendLine(elem.ToString());
                }
                return output.ToString();
            }
        }

        /// <summary>
        ///     Simple threadsafe stack with async Add and TakeAsync
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        public class SafeStackAsync<T> where T : class
        {
            private readonly object _arbitraryLockingObject = new();
            private readonly Stack<T> _mCollection;

            private readonly Stack<TaskCompletionSource<T>> _mWaiting;

            public SafeStackAsync()
            {
                lock (_arbitraryLockingObject)
                {
                    _mCollection = new Stack<T>();
                    _mWaiting = new Stack<TaskCompletionSource<T>>();
                }
            }

            public SafeStackAsync(int capacity)
            {
                lock (_arbitraryLockingObject)
                {
                    _mCollection = new Stack<T>(capacity);
                    _mWaiting = new Stack<TaskCompletionSource<T>>(capacity);
                }
            }


            public void Add(T item)
            {
                TaskCompletionSource<T> tcs = null;

                lock (_arbitraryLockingObject)
                {
                    if (_mWaiting.Count > 0)
                        tcs = _mWaiting.Pop();
                    else
                        _mCollection.Push(item);
                }

                tcs?.TrySetResult(item);
            }

            /// <summary>
            ///     async method
            /// </summary>
            /// <returns>Task typeof T</returns>
            public Task<T> TakeAsync()
            {
                lock (_arbitraryLockingObject)
                {
                    if (_mCollection.Count > 0)
                    {
                        //return Task.FromResult(_mCollection.Pop());

                        // Jgh version - works with silverlight as well 
                        //----------------------------------------------------------------
                        var tcs = new TaskCompletionSource<T>();

                        tcs.TrySetResult(_mCollection.Pop());

                        return tcs.Task;

                        //----------------------------------------------------------------
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<T>();

                        _mWaiting.Push(tcs);

                        return tcs.Task;
                    }
                }
            }

            public bool Any()
            {
                lock (_arbitraryLockingObject)
                {
                    return _mCollection.Any();
                }
            }

            public int Count()
            {
                lock (_arbitraryLockingObject)
                {
                    return _mCollection.Count;
                }
            }

            public T[] ToArrayAndClear()
            {
                var numItems = Count();

                var answer = new T[numItems];

                lock (_arbitraryLockingObject)
                {
                    for (var i = 0; i < numItems; i++)
                    {
                        var safeIndex = i;

                        answer[safeIndex] = TakeAsync().Result;
                    }

                    return answer;
                }
            }

            public List<T> ToListAndClear()
            {
                var numItems = Count();

                var answer = new List<T>(numItems);

                lock (_arbitraryLockingObject)
                {
                    for (var i = 0; i < numItems; i++)
                        answer.Add(TakeAsync().Result);

                    return answer;
                }
            }

            public string PrintAllElements()
            {
                var output = new StringBuilder();

                lock (_arbitraryLockingObject)
                {
                    foreach (var elem in _mCollection)
                        output.AppendLine(elem.ToString());
                }
                return output.ToString();
            }
        }

        #endregion

    }
}