using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class EthernetMonitorParser
{
    public static EthernetMonitor ToEtherInterfaceMonitor(this MikrotikSentence s)
    {
        return new EthernetMonitor
        {
            Name = s.Attributes.GetValueOrDefault("name", string.Empty),
            Status = s.Attributes.GetValueOrDefault("status", string.Empty),
            AutoNegotiation = s.Attributes.GetValueOrDefault("auto-negotiation", string.Empty),
            Rate = s.Attributes.GetValueOrDefault("rate", string.Empty),
            FullDuplex = s.Attributes.GetValueOrDefault("full-duplex", string.Empty),
            TxFlowControl = s.Attributes.GetValueOrDefault("tx-flow-control", string.Empty),
            RxFlowControl = s.Attributes.GetValueOrDefault("rx-flow-control", string.Empty),
            Fec = s.Attributes.GetValueOrDefault("fec", string.Empty),
            Supported = s.Attributes.GetValueOrDefault("supported", string.Empty),

            SfpModulePresent = s.Attributes.GetValueOrDefault("sfp-module-present", string.Empty),
            SfpRxLoss = s.Attributes.GetValueOrDefault("sfp-rx-loss", string.Empty),
            SfpTxFault = s.Attributes.GetValueOrDefault("sfp-tx-fault", string.Empty),

            SfpType = s.Attributes.GetValueOrDefault("sfp-type", string.Empty),
            SfpConnectorType = s.Attributes.GetValueOrDefault("sfp-connector-type", string.Empty),
            SfpEncoding = s.Attributes.GetValueOrDefault("sfp-encoding", string.Empty),

            SfpLinkLengthSm = s.Attributes.GetValueOrDefault("sfp-link-length-sm", "0"),
            SfpLinkLengthOm1 = s.Attributes.GetValueOrDefault("sfp-link-length-om1", "0"),
            SfpLinkLengthOm2 = s.Attributes.GetValueOrDefault("sfp-link-length-om2", "0"),
            SfpLinkLengthOm3 = s.Attributes.GetValueOrDefault("sfp-link-length-om3", "0"),
            SfpLinkLengthCopperActiveOm4 = s.Attributes.GetValueOrDefault("sfp-link-length-copper-active-om4", "0"),

            SfpVendorName = s.Attributes.GetValueOrDefault("sfp-vendor-name", string.Empty),
            SfpVendorPartNumber = s.Attributes.GetValueOrDefault("sfp-vendor-part-number", string.Empty),
            SfpVendorRevision = s.Attributes.GetValueOrDefault("sfp-vendor-revision", string.Empty),
            SfpManufacturingDate = s.Attributes.GetValueOrDefault("sfp-manufacturing-date", string.Empty),

            SfpWavelength = s.Attributes.GetValueOrDefault("sfp-wavelength", "0"),
            SfpDwdmChannelSpacing = s.Attributes.GetValueOrDefault("sfp-dwdm-channel-spacing", "0"),

            SfpTemperature = s.Attributes.GetValueOrDefault("sfp-temperature", "0"),
            SfpSupplyVoltage = s.Attributes.GetValueOrDefault("sfp-supply-voltage", "0"),
            SfpTxBiasCurrent = s.Attributes.GetValueOrDefault("sfp-tx-bias-current", "0"),
            SfpTxPower = s.Attributes.GetValueOrDefault("sfp-tx-power", "0"),
            SfpRxPower = s.Attributes.GetValueOrDefault("sfp-rx-power", "0")
        };   
    }
}
