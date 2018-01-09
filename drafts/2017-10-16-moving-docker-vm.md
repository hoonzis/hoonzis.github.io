---
layout: post
title: Moving the docker VM
date: '2017-10-16T01:07:44.866-07:00'
author: Jan Fajfr
tags:
- Docker, Windows
modified_time: '2017-10-16T01:07:44.866-07:00'
---


Logs:

```
C:\Users\{userId}\AppData\Local\Docker\log.txt
```


Docker settings files:

```
C:\Users\jfajfr\AppData\Roaming\Docker\settings.json
```

Look at the VHDX location:

```
"MobyVhdPathOverride": "D:\\docker-vm\\MobyLinuxVM.vhdx",
```


```
Get-VHD –Path "C:\Users\Public\Documents\Hyper-V\Virtual Hard Disks\MobyLinuxVM.vhdx"
```

```
ComputerName            : JAN-PC
Path                    : c:\users\public\documents\hyper-v\virtual hard disks\mobylinuxvm.vhdx
VhdFormat               : VHDX
VhdType                 : Dynamic
FileSize                : 1849688064
Size                    : 64424509440
MinimumSize             : 64424509440
LogicalSectorSize       : 512
PhysicalSectorSize      : 4096
BlockSize               : 33554432
ParentPath              :
DiskIdentifier          : 9D05702B-6CAB-4A48-8CFC-226745061363
FragmentationPercentage : 3
Alignment               : 1
Attached                : True
DiskNumber              :
Number                  :
```

