## Initial outtake, before a plan even exists, leaving for reference

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

## Budget setup

- K3s cluster: lightweight Kubernetes, less resource demand
- Ingress: Traefik (comes with k3s) as reverse proxy + free TLS (Let’s Encrypt).
- Storage:
    - Local-path provisioner (built-in) for quick persistence.
    - MongoDB in a container with local volume for app state.
- App:
    - Backend (.NET API) in a container (hosted on existing/already pre-paid Cloud VM)
    - Frontend (HTMX) hosted at FTP (exiting/already pre-paid).
- CI/CD: GitHub Actions → push to GHCR → pull/deploy on VM via Helm/ArgoCD (or even simple kubectl apply at first).
- Secrets: Kubernetes Secrets for DB credentials; GitHub Secrets for deploy keys.
- Monitoring/Logging:
    - Start with kubectl logs.
    - Optional: Prometheus + Grafana (Helm) later if needed.
    - Skip Azure Monitor/App Insights to avoid ingestion costs.
- Azure presence: Azure Static Web App (free tier) - host FE?
  
## What this means in practice:
- K3s role: run our .NET API as Deployments + multiple replicas behind an Ingress. K3s gives me rollout, health checks, self-healing, and easy scaling.
- Sessions: Use cookie-based auth (stateless) or an external/session store (e.g., Redis) so any replica can serve the request - this could be critical if I do manage to get to the end goal: build an interactive/updating HTMX page. We will see if we get this far. THE Challenge.
- Persistence: Keep data in MongoDB outside the pods (separate container on the same Cloud VM).
- Caching/queues: run Redis/RabbitMQ as separate containers outside the cluster alongside MongoDB?
- K8s storage: the built-in local-path provisioner in k3s is fine for dev/MVP volumes (e.g., temp uploads), but will not rely on it for critical data long-term. It would be cool to integrate it!
- Ingress & traffic: HTMX calls hit the Ingress, routed to the .NET service; the app talks to MongoDB/Redis over the VM, network—pods remain stateless.
Secrets & config: put connection strings in Kubernetes Secrets; app settings in ConfigMaps/Env; deploy via CI/CD.
- HTMX will live on a Azure Static free tier Web App, it does not require high performance as it mostly relays hypermedia traffic over the TCP

---