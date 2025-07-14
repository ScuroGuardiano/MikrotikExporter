# Mikrotik Prometheus Exporter

Easy configurable, efficient Mikrotik metrics exporter with ARM32, ARM64 and AMD64 support.

![Banner](docs/image.png)

# Features
- Easy to configure, without any configuration files
- Works on AMD64, ARM64 and ARMv7.
- You can set up it on your Mikrotik ARM or x64 router
- Better performance than mktxp, although a little less metrics and doesn't support multi router
- Connection pooling with persistence and concurrency

# Getting started
1. Create a group on Mikrotik device with read and api permissions:
   ```sh
   /user/group/add name="mikrotik-exporter" policy=api,read
   ```
2. Create a user on Mikrotik device with this group:
   ```sh
   /user/add name=mikrotik-exporter group=mikrotik-exporter password=set_securepasswordxd
   ```

## Running options   
Then you have three easy options to run it:

### Docker run
Just type, of course put your own values for envs:
```sh
export MIKROTIK__USERNAME=mikrotik-exporter
export MIKROTIK__PASSWORD=set_securepasswordxd
export MIKROTIK__HOST=192.168.88.1
export MIKROTIK__NAME=just_name_that_will_be_showed_as_router_label

dotnet run https://ghcr.io/scuroguardiano/mikrotikexporter:latest -p 5000:5000
```

### Docker Kompot
Docker kompot is another good and easy way to setup it:
```yml
version: '3.8'

services:
mikrotik-exporter:
image: ghcr.io/scuroguardiano/mikrotikexporter:latest
container_name: mikrotik-exporter
environment:
- MIKROTIK__USERNAME=mikrotik-exporter
- MIKROTIK__PASSWORD=set_securepasswordxd
- MIKROTIK__HOST=192.168.88.1
- MIKROTIK__NAME=just_name_that_will_be_showed_as_router_label
ports:
- "5000:5000"
restart: unless-stopped
```
And just run it with
```sh
docker-compose up -d
```

### Mikrotik Container
You can also run it, just like me, inside Mikrotik Container.

Mikrotik has pretty straightforward instructions how to run it [here](https://help.mikrotik.com/docs/spaces/ROS/pages/84901929/Container)

### Run directly
I do not recommend that but if you want to run this directly you must compile it yourself :)

Clone the code and then:
```sh
dotnet publish MikrotikExporter/MikrotikExpoter.csproj -c Release
```

you will get an executable

## Prometheus config:
Example prometheus jobs:
```yml
  # low frequency of change
  - job_name: 'mikrotik-low'
    scrape_interval: 20s
    static_configs:
      - targets: ['10.21.37.1:2137']
    metrics_path: /metrics/presets/low-freq
    relabel_configs:
      - target_label: job
        replacement: 'mikrotik'

  # medium frequency of change
  - job_name: 'mikrotik-medium'
    scrape_interval: 10s
    static_configs:
      - targets: ['10.21.37.1:2137']
    metrics_path: /metrics/presets/medium-freq
    relabel_configs:
      - target_label: job
        replacement: 'mikrotik'

  # ifaces and system resouces
  - job_name: 'mikrotik-ifaces-resources'
    scrape_interval: 1s
    static_configs:
      - targets: ['10.21.37.1:2137']
    metrics_path: /metrics/ifaces/resources
    relabel_configs:
      - target_label: job
        replacement: 'mikrotik'

  # firewall connections
  - job_name: 'mikrotik-fw-conn'
    scrape_interval: 5s
    static_configs:
      - targets: ['10.21.37.1:2137']
    metrics_path: /metrics/firewall-conn
    relabel_configs:
      - target_label: job
        replacement: 'mikrotik'

  # router info [it never changes]
  - job_name: 'mikrotik-info'
    scrape_interval: 120s
    static_configs:
      - targets: ['10.21.37.1:2137']
    metrics_path: /metrics/info
    relabel_configs:
      - target_label: job
        replacement: 'mikrotik'
```

# Environment variables
| Variable Name              | Description                                                        |
|----------------------------|--------------------------------------------------------------------|
| `MIKROTIK__USERNAME`       | Username for authenticating with the Mikrotik device.              |
| `MIKROTIK__PASSWORD`       | Password for authenticating with the Mikrotik device.              |
| `MIKROTIK__HOST`           | IP address or hostname of the Mikrotik device.                     |
| `MIKROTIK__NAME`           | Label for the router displayed in the exporter.                    |
| `MIKROTIK__CONNECTIONPOOL` | Number of concurrent connections to the Mikrotik API (default: 4). |

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

## List of all available paths:
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


# Development
If you want to contribute to this project, clone the repo and install .NET SDK

You can run project with
```sh
dotnet run --project MikrotikExporter/MikrotikExporter.csproj
```

Project targets NativeAOT so your changes must be NativeAOT friendly.  

Currently, code is somewhat messy, it can get messier soon as I play to make some aggressive optimization of parsing
Mikrotik responses and generating metrics.

Code also have planned support for HTTP, HTTPS and SSL Tcp API but that will be future.

# License
AGPLv3