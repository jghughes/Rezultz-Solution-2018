namespace Rezultz.Library02.Mar2024.DataGridInterfaces
{
    public interface IDataGridRowGroupsHelperService
    {
        void CollapseGroups(bool mustCollapseRatherThanExpand, int groupingLevel, bool doTheSameToAllSublevels = false);

        void CollapseGroups2(bool mustCollapseRatherThanExpand, int groupingLevel,
            bool doTheSameToAllSublevels = false);
    }
}