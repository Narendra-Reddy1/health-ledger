namespace BenStudios.EventSystem
{
    public enum EventID
    {

        OnToggleLoadingPanel,

        //UI 
        OnFloorButtonClicked,
        OnStepsUploadedToSever,
        OnStepsUploadedToBlockchain,
        OnAggregatorErrorEncountered,
        OnJoinTournamentWithoutWallet,


        //Core
        OnStepCountUpdated,
        OnStepCountRecorded,

        
        
        //Android Permissions
        OnActivityPermissionGranted,
        OnActivityPermissionDeclined,


    }
}