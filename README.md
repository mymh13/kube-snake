## This is
A project created by:
Repository can be found here: https://github.com/mymh13/kube-snake

The idea behind it: a project to build a Kubernetes-driven .NET-system
Simple webpage, microservice, CI/CD
See more detail in the "Initial outtake" section below

## Architecture and layout of the system
That can be found in /docs/architecture.md

---

## Initial outtake, before a plan even exists:

1) build a kubernetes cluster - and keep the costs down to a minimum! super important
2) use helm, argocd, persistant storage through the built-in-database but also look at external storage (mongodb)
3) possibly use azure, especially the load balancer, we will decide on storage options later so we develop locally through docker desktop initially but then we plan to deploy it somewhere later
4) we will have a webpage built in .net and running htmx (that is the final section that we will do)
5) the web application needs to store state somewhere, and since we want k8s to be stateless, and we have live updates on the htmx-webpage, we need a plan for a cheap storage - mongodb stored in a container on an already-existing vm I run at hetzner?
6) a ci/cd system, we mentioned helm and argocd, we also want to look at github actions and a scaled trunk development I think?
7) the microservice system: k8s, multiple pods (frontend, backend, replicas) and possibly separated storage where some is at azure and some at hetzner (maybe some at a FTP too)
8) EVERY STEP will be small, incremental, we will verify it! Every single step, sloooow and steady!
9) the 5 most vital points for the project:
- code should be images in containers
- persistance: storage in the built-in db as well as externally in mongodb
- we need at least some kind of functionality in azure
- two means to deploy: helm + argocd, implement those
- ingress - use it like a reverse proxy?
10) secrets:
- github secrets (deploy) and k8s secrets (accounts)
- network policies for pod to pod communication
- rbac for k8s? overkill for a simple webpage perhaps
11) logginggggggggggggggg and testing
- fluentd/fluentbit > azure log analytics?
- metrics & monitoring > prometheus + grafana? azure monitor?
- azure application insights? I want logging + metrics but keep costs and complexity down!
- unit testing? e2e tests of pipeline? too small for TDD this time

---