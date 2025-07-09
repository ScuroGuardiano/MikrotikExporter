using MikrotikApiClient.Dto;

namespace MikrotikApiClient.Tcp.Parsers;

internal static class WlanRegistrationParser
{
    public static WlanRegistration ToWlanRegistration(this MikrotikSentence s)
    {
        return new WlanRegistration
        {
            Id = s.Attributes[".id"],
            Interface = s.Attributes["interface"],
            MacAddress = s.Attributes["mac-address"],

            Ap = s.Attributes.GetValueOrDefault("ap", "false"),
            Wds = s.Attributes.GetValueOrDefault("wds", "false"),
            Bridge = s.Attributes.GetValueOrDefault("bridge", "false"),

            RxRate = s.Attributes.GetValueOrDefault("rx-rate", string.Empty),
            TxRate = s.Attributes.GetValueOrDefault("tx-rate", string.Empty),

            TxPackets = s.Attributes.GetValueOrDefault("packets", "0,0").Split(',')[0],
            RxPackets = s.Attributes.GetValueOrDefault("packets", "0,0").Split(',')[1],

            TxBytes = s.Attributes.GetValueOrDefault("bytes", "0,0").Split(',')[0],
            RxBytes = s.Attributes.GetValueOrDefault("bytes", "0,0").Split(',')[1],

            TxFrames = s.Attributes.GetValueOrDefault("frames", "0,0").Split(',')[0],
            RxFrames = s.Attributes.GetValueOrDefault("frames", "0,0").Split(',')[1],

            TxFrameBytes = s.Attributes.GetValueOrDefault("frame-bytes", "0,0").Split(',')[0],
            RxFrameBytes = s.Attributes.GetValueOrDefault("frame-bytes", "0,0").Split(',')[1],

            TxHwFrames = s.Attributes.GetValueOrDefault("hw-frames", "0,0").Split(',')[0],
            RxHwFrames = s.Attributes.GetValueOrDefault("hw-frames", "0,0").Split(',')[1],

            TxHwFrameBytes = s.Attributes.GetValueOrDefault("hw-frame-bytes", "0,0").Split(',')[0],
            RxHwFrameBytes = s.Attributes.GetValueOrDefault("hw-frame-bytes", "0,0").Split(',')[1],

            TxFramesTimedOut = s.Attributes.GetValueOrDefault("tx-frames-timed-out", "0"),

            Uptime = s.Attributes.GetValueOrDefault("uptime", string.Empty),
            LastActivity = s.Attributes.GetValueOrDefault("last-activity", string.Empty),

            SignalStrength = s.Attributes.GetValueOrDefault("signal-strength", string.Empty),
            SignalToNoise = s.Attributes.GetValueOrDefault("signal-to-noise", string.Empty),

            SignalStrengthCh0 = s.Attributes.GetValueOrDefault("signal-strength-ch0", string.Empty),
            SignalStrengthCh1 = s.Attributes.GetValueOrDefault("signal-strength-ch1", string.Empty),
            SignalStrengthCh2 = s.Attributes.GetValueOrDefault("signal-strength-ch2", string.Empty),
            SignalStrengthCh3 = s.Attributes.GetValueOrDefault("signal-strength-ch3", string.Empty),

            StrengthAtRates = s.Attributes.GetValueOrDefault("strength-at-rates", string.Empty),
            TxCcq = s.Attributes.GetValueOrDefault("tx-ccq", string.Empty),
            PThroughput = s.Attributes.GetValueOrDefault("p-throughput", string.Empty),

            LastIp = s.Attributes.GetValueOrDefault("last-ip", string.Empty),
            Port802_1xEnabled = s.Attributes.GetValueOrDefault("802.1x-port-enabled", "false"),

            AuthenticationType = s.Attributes.GetValueOrDefault("authentication-type", string.Empty),
            Encryption = s.Attributes.GetValueOrDefault("encryption", string.Empty),
            GroupEncryption = s.Attributes.GetValueOrDefault("group-encryption", string.Empty),
            ManagementProtection = s.Attributes.GetValueOrDefault("management-protection", "false"),
            WmmEnabled = s.Attributes.GetValueOrDefault("wmm-enabled", "false"),

            TxRateSet = s.Attributes.GetValueOrDefault("tx-rate-set", string.Empty),
            Comment = s.Attributes.GetValueOrDefault("comment", string.Empty),
        };
    }
}