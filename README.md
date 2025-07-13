# Mikrotik Prometheus Exporter

Easy configurable, efficient Mikrotik metrics exporter with ARM32, ARM64 and AMD64 support.

# Paths
* `/metrics` - every metric available
* `/metrics/presets/low-freq` - metrics with low frequency of change
* `/metrics/presets/medium-freq` - metrics with medium frequency of change
* `/metrics/presets/high-freq` - metrics with high frequency of change
* `/metrics/{*selected_metrics}`, explain below

## Select metrics to export
Exporter allows consumer to select which metrics they are interested in with an easy way. You just add them to path.

Examples:
* `/metrics/ifaces/resources/ether-mon` will return only metrics for interfaces, system resources and ethernet monitor.
* `/metrics/firewall-rules/firewall-conn` will return only metrics for firewall rules and firewall connections.
* `/metrics/ip-pool/ifaces` will return metrics only for ip pools and interfaces

You can use any combination you want. Thanks that you can select which metrics you want to collect and you can even
collect them with different intervals.

### List of all available paths:
| Path                 | Metrics                               | Description                                                                   |
|----------------------|---------------------------------------|-------------------------------------------------------------------------------|
| ifaces               | InterfaceSummaryCollector             | Basic information about interfaces and rx/tx packets/bytes                    |
| ether-mon            | EthernetMonitorCollector              | SFP information like wavelength, rx/tx power, temperature                     |
| wlan-mon             | WlanMonitorCollector                  | Information about WLAN interfaces                                             |
| health               | HealthCollector                       | Metrics like temperature and voltage                                          |
| info                 | RouterInfoCollector                   | Router information, static data, collected infrequently                       |
| resources            | SystemResourceCollector               | Router system resources, like CPU usage                                       |
| dhcp-server-leases   | DhcpServerLeaseCollector              | DHCP server lease information                                                 |
| ip-pool              | IpPoolCollector                       | IP pool information                                                           |
| pppoe-client-mon     | PppoeClientMonitorCollector           | PPPoE client information                                                      |
| wlan-registration    | WlanRegistrationCollector             | Information about connected clients to WLAN                                   |
| firewall-rules       | IpFirewallRuleCollector               | Firewall rule information                                                     |
| firewall-conn        | IpFirewallConnectionCollector         | Firewall connection data, exports a lot but can contain valuable information  |

