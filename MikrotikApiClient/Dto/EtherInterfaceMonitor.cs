namespace MikrotikApiClient.Dto;

public class EtherInternetMonitor
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AutoNegotiation { get; set; } = string.Empty;
    public string Rate { get; set; } = string.Empty;
    public string FullDuplex { get; set; } = string.Empty;
    public string TxFlowControl { get; set; } = string.Empty;
    public string RxFlowControl { get; set; } = string.Empty;
    public string Fec { get; set; } = string.Empty;
    public string Supported { get; set; } = string.Empty;

    public string SfpModulePresent { get; set; } = string.Empty;
    public string SfpRxLoss { get; set; } = string.Empty;
    public string SfpTxFault { get; set; } = string.Empty;

    public string SfpType { get; set; } = string.Empty;
    public string SfpConnectorType { get; set; } = string.Empty;
    public string SfpEncoding { get; set; } = string.Empty;

    public string SfpLinkLengthSm { get; set; } = "0";
    public string SfpLinkLengthOm1 { get; set; } = "0";
    public string SfpLinkLengthOm2 { get; set; } = "0";
    public string SfpLinkLengthOm3 { get; set; } = "0";
    public string SfpLinkLengthCopperActiveOm4 { get; set; } = "0";

    public string SfpVendorName { get; set; } = string.Empty;
    public string SfpVendorPartNumber { get; set; } = string.Empty;
    public string SfpVendorRevision { get; set; } = string.Empty;
    public string SfpManufacturingDate { get; set; } = string.Empty;

    public string SfpWavelength { get; set; } = "0";
    public string SfpDwdmChannelSpacing { get; set; } = "0";

    public string SfpTemperature { get; set; } = "0";
    public string SfpSupplyVoltage { get; set; } = "0";
    public string SfpTxBiasCurrent { get; set; } = "0";
    public string SfpTxPower { get; set; } = "0";
    public string SfpRxPower { get; set; } = "0";
}
