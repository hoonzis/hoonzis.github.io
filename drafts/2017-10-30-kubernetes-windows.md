---
layout: post
title: Kubernetes cluster on Windows
date: '2017-10-16T01:07:44.866-07:00'
author: Jan Fajfr
tags:
- Docker, Windows, Kubernetes
modified_time: '2017-10-16T01:07:44.866-07:00'
---


- Minikube - tool to run a Kubernetes cluster inside a VM on your dev PC
- KubeCtl - command line tool to run queries against your kubernetes cluster
- Docker - manage you containers
- Tool to manage VMs - Docker & Minikube both need a VM so you will need Hyper-V or VirtualBox


λ minikube.exe start --vm-driver="hyperv" --hyperv-virtual-switch=kubeswitch

λ kubectl cluster-info

λ kubectl get nodes

Creating a local registry:

λ docker pull registry

λ docker run -d -p 5000:5000 --restart=always --name registry registry:2




