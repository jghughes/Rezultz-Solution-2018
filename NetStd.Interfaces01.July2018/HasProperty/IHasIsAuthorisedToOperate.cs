namespace NetStd.Interfaces01.July2018.HasProperty
{
    public interface IHasIsAuthorisedToOperate
    {
        bool IsAuthorisedToOperate { get; set; }

        void CaptureIsAuthorisedToOperateValue();

        void RestoreCapturedIsAuthorisedToOperateValue();
    }
}