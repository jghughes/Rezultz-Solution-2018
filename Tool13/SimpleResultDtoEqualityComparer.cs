using Rezultz.DataTransferObjects.Nov2023.Results;

namespace Tool13;

internal class PossiblySuspiciousFinishEqualityComparer : IEqualityComparer<PossiblySuspiciousFinish>
{
    #region IEqualityComparer<Car> Members

    public bool Equals(PossiblySuspiciousFinish? x, PossiblySuspiciousFinish? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.ResultDto.RaceGroup.Equals(y.ResultDto.RaceGroup);
    }

    public int GetHashCode(PossiblySuspiciousFinish obj)
    {
        return obj.ResultDto.RaceGroup.GetHashCode();
    }

    #endregion
}
internal class SimpleResultDtoEqualityComparer : IEqualityComparer<ResultDto>
{
    #region IEqualityComparer<Car> Members

    public bool Equals(ResultDto? x, ResultDto? y)
    {
        if(x == null && y == null)
        {
            return true;
        }

        if(x == null || y == null)
        {
            return false;
        }

        return x.RaceGroup.Equals(y.RaceGroup);
    }

    public int GetHashCode(ResultDto obj)
    {
        return obj.RaceGroup.GetHashCode();
    }

    #endregion
}