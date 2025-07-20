using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Benchmarks.Optimized;

internal sealed class FakeTcpClient : IDisposable, IAsyncDisposable
{
    private const string Data = """
                                AyFyZQc9LmlkPSoyDD1uYW1lPWV0aGVyMRQ9ZGVmYXVsdC1uYW1lPWV0aGVyMQs9dHlwZT1ldGhl
                                cgk9bXR1PTE1MDAQPWFjdHVhbC1tdHU9MTUwMAs9bDJtdHU9MTU5Mg89bWF4LWwybXR1PTk1Nzge
                                PW1hYy1hZGRyZXNzPTc0OjREOjI4OjExOjJGOjQzKD1sYXN0LWxpbmstZG93bi10aW1lPTIwMjUt
                                MDctMTYgMjE6MjY6MjgmPWxhc3QtbGluay11cC10aW1lPTIwMjUtMDctMTYgMjE6MjY6NTgNPWxp
                                bmstZG93bnM9NhQ9cngtYnl0ZT0zMjEzNTM0MTM4OBU9dHgtYnl0ZT0yODUyNzM5NzY0NTkTPXJ4
                                LXBhY2tldD00NzYzMzYxMxQ9dHgtcGFja2V0PTIwNjk1NjU4MhM9dHgtcXVldWUtZHJvcD0xNzg0
                                Fj1mcC1yeC1ieXRlPTg4MDU3ODQ0MDMXPWZwLXR4LWJ5dGU9NTUzMDQ0NjEyMTQWPWZwLXJ4LXBh
                                Y2tldD0xNzIxNzc4OBY9ZnAtdHgtcGFja2V0PTQxMzI1MDk2DT1ydW5uaW5nPXRydWULPXNsYXZl
                                PXRydWUPPWRpc2FibGVkPWZhbHNlAAMhcmUHPS5pZD0qMww9bmFtZT1ldGhlcjIUPWRlZmF1bHQt
                                bmFtZT1ldGhlcjILPXR5cGU9ZXRoZXIJPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1
                                PTE1OTIPPW1heC1sMm10dT05NTc4Hj1tYWMtYWRkcmVzcz03NDo0RDoyODoxMToyRjo0NCg9bGFz
                                dC1saW5rLWRvd24tdGltZT0yMDI1LTA3LTE2IDE3OjMzOjExJj1sYXN0LWxpbmstdXAtdGltZT0y
                                MDI1LTA3LTE2IDE3OjMzOjQwDT1saW5rLWRvd25zPTMUPXJ4LWJ5dGU9NDg1MzIxODI4MDEUPXR4
                                LWJ5dGU9NjgyMTA5NDE0NTMTPXJ4LXBhY2tldD05ODQzODExNBQ9dHgtcGFja2V0PTExMjIwMjc5
                                MhE9dHgtcXVldWUtZHJvcD03Nhc9ZnAtcngtYnl0ZT00MjQ2NTIxOTk0OBY9ZnAtdHgtYnl0ZT02
                                ODQ3MTYwNDkzFj1mcC1yeC1wYWNrZXQ9ODYzNjY2MzYWPWZwLXR4LXBhY2tldD0xMDI4MDEwNA09
                                cnVubmluZz10cnVlCz1zbGF2ZT10cnVlDz1kaXNhYmxlZD1mYWxzZQADIXJlBz0uaWQ9KjQMPW5h
                                bWU9ZXRoZXIzFD1kZWZhdWx0LW5hbWU9ZXRoZXIzCz10eXBlPWV0aGVyCT1tdHU9MTUwMBA9YWN0
                                dWFsLW10dT0xNTAwCz1sMm10dT0xNTkyDz1tYXgtbDJtdHU9OTU3OB49bWFjLWFkZHJlc3M9NzQ6
                                NEQ6Mjg6MTE6MkY6NDUoPWxhc3QtbGluay1kb3duLXRpbWU9MjAyNS0wNy0yMCAxNjo0MTo1NiY9
                                bGFzdC1saW5rLXVwLXRpbWU9MjAyNS0wNy0yMCAxNjo0MjowMw49bGluay1kb3ducz04MxU9cngt
                                Ynl0ZT0yNDQ2OTAxMDA2OTIVPXR4LWJ5dGU9MzA0NDUyNzk4NDI0FD1yeC1wYWNrZXQ9MjMxNjcw
                                MjQyFD10eC1wYWNrZXQ9MjM1MTYxNDE3FD10eC1xdWV1ZS1kcm9wPTQwNzA3Fz1mcC1yeC1ieXRl
                                PTIwMTA1ODE0Njg0GD1mcC10eC1ieXRlPTI5NTM2MTQ3MjMxMxY9ZnAtcngtcGFja2V0PTgyNDUz
                                MjE3Fz1mcC10eC1wYWNrZXQ9MjE4ODExODk4DT1ydW5uaW5nPXRydWULPXNsYXZlPXRydWUPPWRp
                                c2FibGVkPWZhbHNlAAMhcmUHPS5pZD0qNQw9bmFtZT1ldGhlcjQUPWRlZmF1bHQtbmFtZT1ldGhl
                                cjQLPXR5cGU9ZXRoZXIJPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1PTE1OTIPPW1h
                                eC1sMm10dT05NTc4Hj1tYWMtYWRkcmVzcz03NDo0RDoyODoxMToyRjo0Ng09bGluay1kb3ducz0w
                                Cj1yeC1ieXRlPTAKPXR4LWJ5dGU9MAw9cngtcGFja2V0PTAMPXR4LXBhY2tldD0wED10eC1xdWV1
                                ZS1kcm9wPTANPWZwLXJ4LWJ5dGU9MA09ZnAtdHgtYnl0ZT0wDz1mcC1yeC1wYWNrZXQ9MA89ZnAt
                                dHgtcGFja2V0PTAOPXJ1bm5pbmc9ZmFsc2ULPXNsYXZlPXRydWUPPWRpc2FibGVkPWZhbHNlAAMh
                                cmUHPS5pZD0qNgw9bmFtZT1ldGhlcjUUPWRlZmF1bHQtbmFtZT1ldGhlcjULPXR5cGU9ZXRoZXIJ
                                PW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1PTE1OTIPPW1heC1sMm10dT05NTc4Hj1t
                                YWMtYWRkcmVzcz03NDo0RDoyODoxMToyRjo0Nyg9bGFzdC1saW5rLWRvd24tdGltZT0yMDI1LTA3
                                LTE3IDE2OjA2OjI5Jj1sYXN0LWxpbmstdXAtdGltZT0yMDI1LTA3LTE3IDE2OjA1OjUzDT1saW5r
                                LWRvd25zPTEMPXJ4LWJ5dGU9MTI4DT10eC1ieXRlPTEyNDIMPXJ4LXBhY2tldD0yDT10eC1wYWNr
                                ZXQ9MTkQPXR4LXF1ZXVlLWRyb3A9MA89ZnAtcngtYnl0ZT0xMjANPWZwLXR4LWJ5dGU9MA89ZnAt
                                cngtcGFja2V0PTIPPWZwLXR4LXBhY2tldD0wDj1ydW5uaW5nPWZhbHNlCz1zbGF2ZT10cnVlDz1k
                                aXNhYmxlZD1mYWxzZQADIXJlBz0uaWQ9KjcMPW5hbWU9ZXRoZXI2FD1kZWZhdWx0LW5hbWU9ZXRo
                                ZXI2Cz10eXBlPWV0aGVyCT1tdHU9MTUwMBA9YWN0dWFsLW10dT0xNTAwCz1sMm10dT0xNTkyDz1t
                                YXgtbDJtdHU9OTU3OB49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NDgoPWxhc3QtbGluay1k
                                b3duLXRpbWU9MjAyNS0wNy0xNyAxNjowNzowMCY9bGFzdC1saW5rLXVwLXRpbWU9MjAyNS0wNy0x
                                NyAxNjowNjozMw09bGluay1kb3ducz0yDT1yeC1ieXRlPTEzNzANPXR4LWJ5dGU9MjU5Mg09cngt
                                cGFja2V0PTIxDT10eC1wYWNrZXQ9MzEQPXR4LXF1ZXVlLWRyb3A9MBA9ZnAtcngtYnl0ZT0xMjAw
                                DT1mcC10eC1ieXRlPTAQPWZwLXJ4LXBhY2tldD0yMA89ZnAtdHgtcGFja2V0PTAOPXJ1bm5pbmc9
                                ZmFsc2ULPXNsYXZlPXRydWUPPWRpc2FibGVkPWZhbHNlAAMhcmUHPS5pZD0qOAw9bmFtZT1ldGhl
                                cjcUPWRlZmF1bHQtbmFtZT1ldGhlcjcLPXR5cGU9ZXRoZXIJPW10dT0xNTAwED1hY3R1YWwtbXR1
                                PTE1MDALPWwybXR1PTE1OTIPPW1heC1sMm10dT05NTc4Hj1tYWMtYWRkcmVzcz03NDo0RDoyODox
                                MToyRjo0OQ09bGluay1kb3ducz0wCj1yeC1ieXRlPTAKPXR4LWJ5dGU9MAw9cngtcGFja2V0PTAM
                                PXR4LXBhY2tldD0wED10eC1xdWV1ZS1kcm9wPTANPWZwLXJ4LWJ5dGU9MA09ZnAtdHgtYnl0ZT0w
                                Dz1mcC1yeC1wYWNrZXQ9MA89ZnAtdHgtcGFja2V0PTAOPXJ1bm5pbmc9ZmFsc2ULPXNsYXZlPXRy
                                dWUPPWRpc2FibGVkPWZhbHNlAAMhcmUHPS5pZD0qOQw9bmFtZT1ldGhlcjgUPWRlZmF1bHQtbmFt
                                ZT1ldGhlcjgLPXR5cGU9ZXRoZXIJPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1PTE1
                                OTIPPW1heC1sMm10dT05NTc4Hj1tYWMtYWRkcmVzcz03NDo0RDoyODoxMToyRjo0QQ09bGluay1k
                                b3ducz0wCj1yeC1ieXRlPTAKPXR4LWJ5dGU9MAw9cngtcGFja2V0PTAMPXR4LXBhY2tldD0wED10
                                eC1xdWV1ZS1kcm9wPTANPWZwLXJ4LWJ5dGU9MA09ZnAtdHgtYnl0ZT0wDz1mcC1yeC1wYWNrZXQ9
                                MA89ZnAtdHgtcGFja2V0PTAOPXJ1bm5pbmc9ZmFsc2ULPXNsYXZlPXRydWUPPWRpc2FibGVkPWZh
                                bHNlAAMhcmUHPS5pZD0qQQw9bmFtZT1ldGhlcjkUPWRlZmF1bHQtbmFtZT1ldGhlcjkLPXR5cGU9
                                ZXRoZXIJPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1PTE1OTIPPW1heC1sMm10dT05
                                NTc4Hj1tYWMtYWRkcmVzcz03NDo0RDoyODoxMToyRjo0Qig9bGFzdC1saW5rLWRvd24tdGltZT0y
                                MDI1LTA3LTE3IDE2OjA3OjAwJj1sYXN0LWxpbmstdXAtdGltZT0yMDI1LTA3LTE3IDE2OjA2OjM3
                                DT1saW5rLWRvd25zPTENPXJ4LWJ5dGU9MjQ2NAw9dHgtYnl0ZT0xMjgNPXJ4LXBhY2tldD0yOQw9
                                dHgtcGFja2V0PTIQPXR4LXF1ZXVlLWRyb3A9MA89ZnAtcngtYnl0ZT03ODANPWZwLXR4LWJ5dGU9
                                MBA9ZnAtcngtcGFja2V0PTEzDz1mcC10eC1wYWNrZXQ9MA49cnVubmluZz1mYWxzZQs9c2xhdmU9
                                dHJ1ZQ89ZGlzYWJsZWQ9ZmFsc2UAAyFyZQc9LmlkPSpCDT1uYW1lPWV0aGVyMTAVPWRlZmF1bHQt
                                bmFtZT1ldGhlcjEwCz10eXBlPWV0aGVyCT1tdHU9MTUwMBA9YWN0dWFsLW10dT0xNTAwCz1sMm10
                                dT0xNTkyDz1tYXgtbDJtdHU9OTU3OB49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NEMNPWxp
                                bmstZG93bnM9MAo9cngtYnl0ZT0wCj10eC1ieXRlPTAMPXJ4LXBhY2tldD0wDD10eC1wYWNrZXQ9
                                MBA9dHgtcXVldWUtZHJvcD0wDT1mcC1yeC1ieXRlPTANPWZwLXR4LWJ5dGU9MA89ZnAtcngtcGFj
                                a2V0PTAPPWZwLXR4LXBhY2tldD0wDj1ydW5uaW5nPWZhbHNlCz1zbGF2ZT10cnVlDz1kaXNhYmxl
                                ZD1mYWxzZQADIXJlBz0uaWQ9KkMSPW5hbWU9c2ZwLXNmcHBsdXMxGj1kZWZhdWx0LW5hbWU9c2Zw
                                LXNmcHBsdXMxCz10eXBlPWV0aGVyCT1tdHU9MTUwMBA9YWN0dWFsLW10dT0xNTAwCz1sMm10dT0x
                                NTkyDz1tYXgtbDJtdHU9OTU4Nh49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NEQmPWxhc3Qt
                                bGluay11cC10aW1lPTIwMjUtMDctMDcgMDA6MDY6MTcNPWxpbmstZG93bnM9MBU9cngtYnl0ZT00
                                NzYyNTI0MDYzNjAUPXR4LWJ5dGU9Njg0ODg1MDMxNzEUPXJ4LXBhY2tldD0zNzgyOTQ3MzkUPXR4
                                LXBhY2tldD0xOTc4NDM1NjIKPXJ4LWRyb3A9MAo9dHgtZHJvcD0wEz10eC1xdWV1ZS1kcm9wPTI0
                                NzgLPXJ4LWVycm9yPTALPXR4LWVycm9yPTAYPWZwLXJ4LWJ5dGU9NDc2MjUyNDA2MzYwFz1mcC10
                                eC1ieXRlPTY4NDg4NTAzMTcxFz1mcC1yeC1wYWNrZXQ9Mzc4Mjk0NzM5Fz1mcC10eC1wYWNrZXQ9
                                MTk3ODQzNTYyDT1ydW5uaW5nPXRydWUPPWRpc2FibGVkPWZhbHNlAAMhcmUHPS5pZD0qRAs9bmFt
                                ZT13bGFuMRM9ZGVmYXVsdC1uYW1lPXdsYW4xCj10eXBlPXdsYW4JPW10dT0xNTAwED1hY3R1YWwt
                                bXR1PTE1MDALPWwybXR1PTE2MDAPPW1heC1sMm10dT0yMjkwHj1tYWMtYWRkcmVzcz1CODo2OTpG
                                NDpCMTpBRDpFNSg9bGFzdC1saW5rLWRvd24tdGltZT0yMDI1LTA3LTE5IDE2OjU1OjU1Jj1sYXN0
                                LWxpbmstdXAtdGltZT0yMDI1LTA3LTE5IDE2OjU0OjMwDj1saW5rLWRvd25zPTMzED1yeC1ieXRl
                                PTQxNTg2OTMRPXR4LWJ5dGU9Mzc4ODQwNzQQPXJ4LXBhY2tldD0xODkzOBA9dHgtcGFja2V0PTQx
                                MjcxCj1yeC1kcm9wPTAKPXR4LWRyb3A9MBA9dHgtcXVldWUtZHJvcD0wCz1yeC1lcnJvcj0wCz10
                                eC1lcnJvcj0wEz1mcC1yeC1ieXRlPTQxNTg2OTMUPWZwLXR4LWJ5dGU9MzY5NjU5OTgTPWZwLXJ4
                                LXBhY2tldD0xODkzOBM9ZnAtdHgtcGFja2V0PTMzNzk4Dj1ydW5uaW5nPWZhbHNlCz1zbGF2ZT10
                                cnVlDz1kaXNhYmxlZD1mYWxzZQ89Y29tbWVudD0yLjRHSHoAAyFyZQc9LmlkPSpFCz1uYW1lPXds
                                YW4yEz1kZWZhdWx0LW5hbWU9d2xhbjIKPXR5cGU9d2xhbgk9bXR1PTE1MDAQPWFjdHVhbC1tdHU9
                                MTUwMAs9bDJtdHU9MTYwMA89bWF4LWwybXR1PTIyOTAePW1hYy1hZGRyZXNzPTc0OjREOjI4OjEx
                                OjJGOjRFKD1sYXN0LWxpbmstZG93bi10aW1lPTIwMjUtMDctMTkgMTY6NTY6MTkmPWxhc3QtbGlu
                                ay11cC10aW1lPTIwMjUtMDctMTkgMTY6NTU6NTIOPWxpbmstZG93bnM9NDAUPXJ4LWJ5dGU9NTE3
                                NDgzMzk4NzMVPXR4LWJ5dGU9MTM4MDY2OTUzMDI2Ez1yeC1wYWNrZXQ9ODgxOTE4MjcUPXR4LXBh
                                Y2tldD0xMDY4MTgwNDEKPXJ4LWRyb3A9MAo9dHgtZHJvcD0wFT10eC1xdWV1ZS1kcm9wPTM2MjIy
                                OAs9cngtZXJyb3I9MAs9dHgtZXJyb3I9MBc9ZnAtcngtYnl0ZT01MTc0ODMzOTg3Mxg9ZnAtdHgt
                                Ynl0ZT0xMzY3NDUwOTE4MTYWPWZwLXJ4LXBhY2tldD04ODE5MTgyNxc9ZnAtdHgtcGFja2V0PTEw
                                NDQxODY5Mw49cnVubmluZz1mYWxzZQs9c2xhdmU9dHJ1ZQ89ZGlzYWJsZWQ9ZmFsc2UNPWNvbW1l
                                bnQ9NUdIegADIXJlCD0uaWQ9KjE0Cz1uYW1lPXdsYW4zCj10eXBlPXdsYW4JPW10dT0xNTAwED1h
                                Y3R1YWwtbXR1PTE1MDALPWwybXR1PTE2MDAPPW1heC1sMm10dT0yMjkwHj1tYWMtYWRkcmVzcz1C
                                QTo2OTpGNDpCMTpBRDpFNQ09bGluay1kb3ducz0wCj1yeC1ieXRlPTAMPXR4LWJ5dGU9NjM2DD1y
                                eC1wYWNrZXQ9MAw9dHgtcGFja2V0PTYKPXJ4LWRyb3A9MAo9dHgtZHJvcD0wED10eC1xdWV1ZS1k
                                cm9wPTELPXJ4LWVycm9yPTALPXR4LWVycm9yPTANPWZwLXJ4LWJ5dGU9MA09ZnAtdHgtYnl0ZT0w
                                Dz1mcC1yeC1wYWNrZXQ9MA89ZnAtdHgtcGFja2V0PTAOPXJ1bm5pbmc9ZmFsc2UPPWRpc2FibGVk
                                PWZhbHNlGD1jb21tZW50PVByb3BhZ2FuZGEgU1NJRAADIXJlCD0uaWQ9KjE2Cz1uYW1lPXdsYW40
                                Cj10eXBlPXdsYW4JPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDALPWwybXR1PTE2MDAPPW1heC1s
                                Mm10dT0yMjkwHj1tYWMtYWRkcmVzcz1CQTo2OTpGNDpCMTpBRDpFNig9bGFzdC1saW5rLWRvd24t
                                dGltZT0yMDI1LTA3LTE4IDE2OjE3OjEzJj1sYXN0LWxpbmstdXAtdGltZT0yMDI1LTA3LTE4IDE2
                                OjE3OjE0DT1saW5rLWRvd25zPTYRPXJ4LWJ5dGU9NTcyNjA4MDQRPXR4LWJ5dGU9MTEyNTI5MTMR
                                PXJ4LXBhY2tldD0zMTgyMTIRPXR4LXBhY2tldD0xNjcyNTQKPXJ4LWRyb3A9MAo9dHgtZHJvcD0w
                                ED10eC1xdWV1ZS1kcm9wPTELPXJ4LWVycm9yPTALPXR4LWVycm9yPTAUPWZwLXJ4LWJ5dGU9NTcy
                                NjA4MDQTPWZwLXR4LWJ5dGU9MjE5OTcyMxQ9ZnAtcngtcGFja2V0PTMxODIxMhM9ZnAtdHgtcGFj
                                a2V0PTE3NzQ2DT1ydW5uaW5nPXRydWUPPWRpc2FibGVkPWZhbHNlFT1jb21tZW50PVRVWUEgRGV2
                                aWNlcwADIXJlBz0uaWQ9KkYMPW5hbWU9YnJpZGdlDD10eXBlPWJyaWRnZQk9bXR1PWF1dG8QPWFj
                                dHVhbC1tdHU9MTUwMAs9bDJtdHU9MTU5Mh49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NDQm
                                PWxhc3QtbGluay11cC10aW1lPTIwMjUtMDctMDcgMDA6MDU6NTkNPWxpbmstZG93bnM9MBU9cngt
                                Ynl0ZT0xMjEyODQ0NDMzNjQVPXR4LWJ5dGU9NTM5NTc4NTcxODA1FD1yeC1wYWNrZXQ9MjYyNTIz
                                NDg2FD10eC1wYWNrZXQ9NDUyNTAxOTk4Cj1yeC1kcm9wPTAKPXR4LWRyb3A9MBA9dHgtcXVldWUt
                                ZHJvcD0wCz1yeC1lcnJvcj0wCz10eC1lcnJvcj0wGD1mcC1yeC1ieXRlPTEyMTI2NjE3MjI2NBg9
                                ZnAtdHgtYnl0ZT00OTQwNzk0OTIyMDEXPWZwLXJ4LXBhY2tldD0yNjIzNTMwMjgXPWZwLXR4LXBh
                                Y2tldD0zNjQyNDc0NDQOPWR5bmFtaWM9ZmFsc2UNPXJ1bm5pbmc9dHJ1ZQ89ZGlzYWJsZWQ9ZmFs
                                c2UQPWNvbW1lbnQ9ZGVmY29uZgADIXJlBz0uaWQ9KjEIPW5hbWU9bG8OPXR5cGU9bG9vcGJhY2sK
                                PW10dT02NTUzNhE9YWN0dWFsLW10dT02NTUzNh49bWFjLWFkZHJlc3M9MDA6MDA6MDA6MDA6MDA6
                                MDAmPWxhc3QtbGluay11cC10aW1lPTIwMjUtMDctMDcgMDA6MDU6NTkNPWxpbmstZG93bnM9MBE9
                                cngtYnl0ZT03NzM3MDkyMRE9dHgtYnl0ZT03NzM3MDkyMRE9cngtcGFja2V0PTU1MzY4NRE9dHgt
                                cGFja2V0PTU1MzY4NQo9cngtZHJvcD0wCj10eC1kcm9wPTAQPXR4LXF1ZXVlLWRyb3A9MAs9cngt
                                ZXJyb3I9MAs9dHgtZXJyb3I9MA09ZnAtcngtYnl0ZT0wDT1mcC10eC1ieXRlPTAPPWZwLXJ4LXBh
                                Y2tldD0wDz1mcC10eC1wYWNrZXQ9MA09cnVubmluZz10cnVlDz1kaXNhYmxlZD1mYWxzZQADIXJl
                                CD0uaWQ9KjEyFj1uYW1lPXBmc2Vuc2UtdmxhbjIxMzcKPXR5cGU9dmxhbgk9bXR1PTE1MDAQPWFj
                                dHVhbC1tdHU9MTUwMAs9bDJtdHU9MTU4OB49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NDQm
                                PWxhc3QtbGluay11cC10aW1lPTIwMjUtMDctMDcgMDA6MDU6NTkNPWxpbmstZG93bnM9MBM9cngt
                                Ynl0ZT04Njg5NTQ3MjU2FD10eC1ieXRlPTUxMjQzNjQ2OTE2Ez1yeC1wYWNrZXQ9MTY1ODEyODcT
                                PXR4LXBhY2tldD00MjUwMzMyNQo9cngtZHJvcD0wCj10eC1kcm9wPTAQPXR4LXF1ZXVlLWRyb3A9
                                MAs9cngtZXJyb3I9MAs9dHgtZXJyb3I9MBY9ZnAtcngtYnl0ZT04Njg5NTQ1MDM0Fz1mcC10eC1i
                                eXRlPTUwNzQxMjcyOTY3Fj1mcC1yeC1wYWNrZXQ9MTY1ODEyNTMWPWZwLXR4LXBhY2tldD0zODM2
                                NDY5OQ09cnVubmluZz10cnVlDz1kaXNhYmxlZD1mYWxzZRU9Y29tbWVudD1QZlNlbnNlIFZMQU4A
                                AyFyZQg9LmlkPSoxMQw9bmFtZT1zb3duZXQPPXR5cGU9cHBwb2Utb3V0CT1tdHU9MTQ5MhA9YWN0
                                dWFsLW10dT0xNDkyJj1sYXN0LWxpbmstdXAtdGltZT0yMDI1LTA3LTA3IDAwOjA2OjE3DT1saW5r
                                LWRvd25zPTAVPXJ4LWJ5dGU9NDY2NDAxNzcxOTQyFD10eC1ieXRlPTYzMzMwNjY0MzA4FD1yeC1w
                                YWNrZXQ9Mzc3OTAwNzg4FD10eC1wYWNrZXQ9MTk3MzU1MDA1Cj1yeC1kcm9wPTAKPXR4LWRyb3A9
                                MRA9dHgtcXVldWUtZHJvcD0wCz1yeC1lcnJvcj0wCz10eC1lcnJvcj0wGD1mcC1yeC1ieXRlPTQ2
                                NjM5Mzk1NDQ4Nhc9ZnAtdHgtYnl0ZT0zMTM1NjMyNTgyNhc9ZnAtcngtcGFja2V0PTM3Nzc2MTU0
                                ORc9ZnAtdHgtcGFja2V0PTEzNjkxMTk1OQ89aW5hY3RpdmU9ZmFsc2UNPXJ1bm5pbmc9dHJ1ZQ89
                                ZGlzYWJsZWQ9ZmFsc2UPPWNvbW1lbnQ9U293bmV0AAMhcmUIPS5pZD0qMTAUPW5hbWU9c293bmV0
                                LXZsYW4zMDAKPXR5cGU9dmxhbgk9bXR1PTE0OTIQPWFjdHVhbC1tdHU9MTQ5Mgs9bDJtdHU9MTU4
                                OB49bWFjLWFkZHJlc3M9NzQ6NEQ6Mjg6MTE6MkY6NEQmPWxhc3QtbGluay11cC10aW1lPTIwMjUt
                                MDctMDcgMDA6MDY6MTcNPWxpbmstZG93bnM9MBU9cngtYnl0ZT00NzQ3MzkyMjc0MDQUPXR4LWJ5
                                dGU9Njc2ODU4NjU2ODgUPXJ4LXBhY2tldD0zNzgyOTQ3MzkUPXR4LXBhY2tldD0xOTc3NDg4MTgK
                                PXJ4LWRyb3A9MAo9dHgtZHJvcD0xED10eC1xdWV1ZS1kcm9wPTALPXJ4LWVycm9yPTALPXR4LWVy
                                cm9yPTAYPWZwLXJ4LWJ5dGU9NDc0NzM5MjI3NDA0Fz1mcC10eC1ieXRlPTM0MzY4Mzg4OTI0Fz1m
                                cC1yeC1wYWNrZXQ9Mzc4Mjk0NzM5Fz1mcC10eC1wYWNrZXQ9MTM2OTExOTU5DT1ydW5uaW5nPXRy
                                dWUPPWRpc2FibGVkPWZhbHNlGD1jb21tZW50PVNvd25ldCBWTEFOIDMwMAADIXJlCD0uaWQ9KjE3
                                Cz1uYW1lPXZldGgxCj10eXBlPXZldGgJPW10dT0xNTAwED1hY3R1YWwtbXR1PTE1MDAePW1hYy1h
                                ZGRyZXNzPTk2OkQ5OjFGOjg1OjcwOjE2KD1sYXN0LWxpbmstZG93bi10aW1lPTIwMjUtMDctMTMg
                                MjM6MjU6MjkmPWxhc3QtbGluay11cC10aW1lPTIwMjUtMDctMTMgMjM6MjY6NTUOPWxpbmstZG93
                                bnM9MTcUPXJ4LWJ5dGU9MTIxNzc0Nzk0MTEUPXR4LWJ5dGU9MTk5MDY2NjUxOTMTPXJ4LXBhY2tl
                                dD0zMjM2ODg2NxM9dHgtcGFja2V0PTI2OTA0NTgyCj1yeC1kcm9wPTAKPXR4LWRyb3A9MBA9dHgt
                                cXVldWUtZHJvcD0wCz1yeC1lcnJvcj0wCz10eC1lcnJvcj0wDT1mcC1yeC1ieXRlPTANPWZwLXR4
                                LWJ5dGU9MA89ZnAtcngtcGFja2V0PTAPPWZwLXR4LXBhY2tldD0wDT1ydW5uaW5nPXRydWUPPWRp
                                c2FibGVkPWZhbHNlAAMhcmUIPS5pZD0qMTUPPW5hbWU9emVyb3RpZXIxDj10eXBlPXplcm90aWVy
                                CT1tdHU9MjgwMBA9YWN0dWFsLW10dT0yODAwHj1tYWMtYWRkcmVzcz04RTo2MzpFMTo3QzpDMjo5
                                NCg9bGFzdC1saW5rLWRvd24tdGltZT0yMDI1LTA3LTIzIDAyOjMxOjExJj1sYXN0LWxpbmstdXAt
                                dGltZT0yMDI1LTA3LTA3IDAwOjA2OjM1DT1saW5rLWRvd25zPTAKPXJ4LWJ5dGU9MBE9dHgtYnl0
                                ZT02MjcwNzk2NAw9cngtcGFja2V0PTARPXR4LXBhY2tldD05NzMwNDcKPXJ4LWRyb3A9MAo9dHgt
                                ZHJvcD0wED10eC1xdWV1ZS1kcm9wPTALPXJ4LWVycm9yPTALPXR4LWVycm9yPTANPWZwLXJ4LWJ5
                                dGU9MA09ZnAtdHgtYnl0ZT0wDz1mcC1yeC1wYWNrZXQ9MA89ZnAtdHgtcGFja2V0PTANPXJ1bm5p
                                bmc9dHJ1ZQs9c2xhdmU9dHJ1ZQ89ZGlzYWJsZWQ9ZmFsc2USPWNvbW1lbnQ9bWFpbmluZnJhAAUh
                                ZG9uZQAK
                                """;
    
    private readonly MemoryStream _stream = new MemoryStream(Convert.FromBase64String(Data));
    
    public Stream GetStream() => this._stream;

    public void Rst()
    {
        _stream.Seek(0, SeekOrigin.Begin);
    }

    public void Dispose()
    {
        _stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
    }
}

internal sealed class MikrotikFakeTcpApiConnection : IDisposable
{
    private readonly ILogger? _logger;

    private readonly byte[] _readLenBuffer = new byte[4];
    private readonly byte[] _writeLenBuffer = new byte[5];
    private bool _running = false;

    private readonly FakeTcpClient _client;
    
    public MikrotikFakeTcpApiConnection(ILogger? logger = null)
    {
        _logger = logger;
        _client = new FakeTcpClient();
    }


    public async Task EnsureRunning(CancellationToken cancellationToken = default)
    {
        if (!_running)
        {
            await Start(cancellationToken);
        }
    }

    private async Task Start(CancellationToken cancellationToken = default)
    {
        await Login(cancellationToken);
        _running = true;
    }

    private Task Login(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    public async Task<MikrotikResponse> Request(string[] sentence, CancellationToken cancellationToken = default)
    {
        _client.Rst();
        if (sentence.Length == 0)
        {
            throw new Exception("Sentence is empty");
        }
        
        await WriteSentence(sentence, cancellationToken);
        List<MikrotikSentence> sentences = [];

        for (var resSentence = await ReadSentence(cancellationToken);
             !resSentence.IsDone;
             resSentence = await ReadSentence(cancellationToken))
        {
            sentences.Add(resSentence);
        }

        return new MikrotikResponse
        {
            Sentences = sentences,
            ContainsErrors = sentences.Any(s => s.IsError),
            RequestSentence = sentence,
        };
    }

    private async Task WriteSentence(IEnumerable<string> words, CancellationToken cancellationToken)
    {
        foreach (var word in words)
        {
            await WriteWord(word, cancellationToken);
        }

        // Every sentence ends with an empty word
        await WriteWord("", cancellationToken);
    }
    
    private async Task WriteWord(string word, CancellationToken cancellationToken = default)
    {
        await WriteLength((uint)word.Length, cancellationToken);
        await WriteString(word, cancellationToken);
    }

    private async Task<MikrotikSentence> ReadSentence(CancellationToken cancellationToken = default)
    {
        // Senteces are relatively small, 4kB should be enough to read them
        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        int idx = 0;

        try
        {
            // First word is reply, I will pass here empty memory, so it will give me it just as byte array.
            var reply = await ReadWord(Memory<byte>.Empty, cancellationToken);
            var replyStr = Encoding.UTF8.GetString(reply.Item2!); // It should never be null in that case
            
            // Next words will be placed in previously allocated buffer
            while (true)
            {
                //                                                                -1 for end-of-word 0x00 byte \/
                var (written, overflowed) = await ReadWord(buffer.AsMemory(idx, buffer.Length - idx - 1), cancellationToken);
                
                // End of sentence
                if (written == 0 && overflowed == null)
                {
                    break;
                }

                // I don't think this path should be ever reached, 4KB should be enough for any mikrotik api sentence
                // But just in case I add it here...
                if (overflowed is not null)
                {
                    // Here is the funny part. That means our buffer is not sufficient.
                    // We now must allocate new buffer, copy data from old to new one
                    // And copy additional returned bytes. What a mess.
                    
                    var newRequiredLength = overflowed.Length + written;
                    
                    if (newRequiredLength > 1024 * 1024)
                    {
                        // What the fuck, single sentence should never be that big, throw the fuck out
                        throw new Exception("Sentence is suspiciously large, stopping parse, as it may lead to OOM exceptions on weaker systems");
                    }
                    
                    // Allocate twice as large to avoid reallocating too oftes
                    var newBuffer = ArrayPool<byte>.Shared.Rent(newRequiredLength * 2); 
                    try
                    {
                        buffer.CopyTo(newBuffer, 0);
                        overflowed.CopyTo(newBuffer, idx);
                        idx += overflowed.Length;
                        ArrayPool<byte>.Shared.Return(buffer);
                        buffer = newBuffer;
                        buffer[idx++] = 0x00; // I will mark end of each word with 0x00, hopefully Mikrotik doesn't use it anywhere
                        continue;
                    }
                    catch
                    {
                        ArrayPool<byte>.Shared.Return(newBuffer);
                        throw;
                    }
                    
                }
                
                // And now the happy path!
                idx += written;
                buffer[idx++] = 0x00; // But wait! What if we overflow here? Ha! We can't, trust me. UwU.
                // If you don't trust, just look higher
            };

            return new MikrotikSentence(replyStr, buffer, idx);
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(buffer);
            throw;
        }
    }

    /// <summary>
    /// Reads a word. To optimize this method and avoid useless allocations I made it weird.
    ///
    /// It tries to read data from stream to the target memory, if that suceeds it will return tuple: (writtenBytes, null)<br/>
    /// However, in case when target is not sufficient, it will allocate byte array for this word and return (0, allocatedArray).
    /// </summary>
    /// <param name="target">Target memory to write to</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <ul>
    ///         <li>Tuple (writtenBytes, null) if data has been written to provided target</li>
    ///         <li>Tuple (0, byte[]) if there was no enough space in provied target</li>
    ///         <li>Tuple (0, null) in case of empty word, which is the end of the sentence</li>
    ///     </ul>
    /// </returns>
    private async Task<(int, byte[]?)> ReadWord(Memory<byte> target, CancellationToken cancellationToken = default)
    {
        var len = (int) await ReadLength(cancellationToken);

        if (len == 0)
        {
            return (len, null);
        }

        // This is our special case when there's no room in target
        // In this situation we have no other choice but to allocate buffer for this memory
        // and return it.
        if (target.Length < len)
        {
            var buffer = new byte[len];
            await _client.GetStream().ReadExactlyAsync(buffer, 0, len, cancellationToken);
            return (0, buffer);
        }
        
        await _client.GetStream().ReadExactlyAsync(target[..len], cancellationToken);
        return (len, null);
    }

    private Task OpenSocket(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private void CloseSocket(CancellationToken cancellationToken = default)
    {
        // Nothing Xpp
    }
    
    private Task WriteBytes(byte[] bytes, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    private Task WriteBytes(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private async Task<uint> ReadLength(CancellationToken cancellationToken = default)
    {
        await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 1, cancellationToken);
        
        uint c = _readLenBuffer[0];
        
        if ((c & 0x80) == 0x00)
        {
            return c;
        }

        if ((c & 0xC0) == 0x80)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 1, cancellationToken);
            
            const uint x = 0xC0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
        }
        else if ((c & 0xE0) == 0xC0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 2, cancellationToken);
            
            const uint x = 0xE0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
        }
        else if ((c & 0xF0) == 0xE0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 3, cancellationToken);
            
            const uint x = 0xF0;
            c &= ~x;
            c <<= 8;
            c += _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
            c <<= 8;
            c += _readLenBuffer[2];
        }
        else if ((c & 0xF8) == 0xF0)
        {
            await _client.GetStream().ReadExactlyAsync(_readLenBuffer, 0, 4, cancellationToken);
            c = _readLenBuffer[0];
            c <<= 8;
            c += _readLenBuffer[1];
            c <<= 8;
            c += _readLenBuffer[2];
            c <<= 8;
            c += _readLenBuffer[3];
        }

        return c;
    }

    /// <summary>
    /// Length encoding according to Mikrotik API docs
    /// </summary>
    /// <param name="length"></param>
    /// <param name="cancellationToken"></param>
    private async Task WriteLength(uint length, CancellationToken cancellationToken = default)
    {
        switch (length)
        {
            case < 0x80:
                _writeLenBuffer[0] = (byte)(length);
                await WriteBytes(_writeLenBuffer.AsMemory(0, 1), cancellationToken);
                break;
            case < 0x4000:
            {
                length |= 0x8000;
                uint b1 = (length >> 8) & 0xFF;
                uint b2 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 2), cancellationToken);
                break;
            }
            case < 0x200000:
            {
                length |= 0xC00000;
                uint b1 = (length >> 16) & 0xFF;
                uint b2 = (length >> 8) & 0xFF;
                uint b3 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 3), cancellationToken);
                break;
            }
            case < 0x10000000:
            {
                length |= 0xE0000000;
                uint b1 = (length >> 24) & 0xFF;
                uint b2 = (length >> 16) & 0xFF;
                uint b3 = (length >> 8) & 0xFF;
                uint b4 = length & 0xFF;
            
                _writeLenBuffer[0] = (byte)(b1);
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
                _writeLenBuffer[3] = (byte)(b4);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 4), cancellationToken);
                break;
            }
            default:
            {
                const byte b1 = 0xF0;
                uint b2 = (length >> 24) & 0xFF;
                uint b3 = (length >> 16) & 0xFF;
                uint b4 = (length >> 8) & 0xFF;
                uint b5 = length & 0xFF;

                _writeLenBuffer[0] = b1;
                _writeLenBuffer[1] = (byte)(b2);
                _writeLenBuffer[2] = (byte)(b3);
                _writeLenBuffer[3] = (byte)(b4);
                _writeLenBuffer[4] = (byte)(b5);
            
                await WriteBytes(_writeLenBuffer.AsMemory(0, 5), cancellationToken);
                break;
            }
        }
    }
    
    private async Task WriteString(string s, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        await WriteBytes(bytes, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}