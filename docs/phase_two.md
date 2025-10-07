# Phase 2: GitOps Foundation & MongoDB

## Goal
Implement Helm and ArgoCD for GitOps-based deployments, then add MongoDB as the first stateful workload managed by ArgoCD.

## Philosophy
- Get deployment tooling (Helm + ArgoCD) in place **before** adding complex applications
- Use simple apps (healthcheck) to learn Helm/ArgoCD patterns
- MongoDB becomes the first "real" workload deployed via GitOps
- CI builds images, ArgoCD handles deployments (separation of concerns)

## Steps

### Phase 2A: GitOps Foundation (Helm + ArgoCD)

#### 2.1: Convert Healthcheck to Helm Chart
- [x] Create Helm chart structure (`helm/charts/healthcheck/`)
- [x] Create `Chart.yaml` with metadata
- [x] Create `values.yaml` with configurable parameters (2 replicas)
- [x] Template-ize deployment and service manifests
- [x] Update GitHub Actions workflow to use `helm upgrade --install`
- [x] Test: Deploy via Helm with SHA tag override
- [x] Verify: Healthcheck working with 2 replicas via Helm deployment
- [x] Learn: Helm templating, values override, chart structure
- [x] Security: Fixed SSH key logging issue with `log-public-key: false`

#### 2.2: Install ArgoCD in K3s
- [x] Deploy ArgoCD to K3s cluster
- [x] Access ArgoCD UI via Caddy subdomain (argocd.kube-snake.mymh.dev)
- [x] Configure initial admin access
- [x] Verify: ArgoCD dashboard accessible and repo connected
- [x] Learn: ArgoCD architecture, polling mechanisms, GitOps workflow
- [x] Security: Added security headers to Caddy for ArgoCD subdomain

#### 2.3: ArgoCD-Managed Healthcheck
- [x] Create ArgoCD Application manifest for healthcheck
- [x] Point ArgoCD to healthcheck Helm chart in repo
- [x] Configure auto-sync policy (prune: true, selfHeal: true)
- [x] Test: Git push → ArgoCD detects change → auto-sync → K3s deploy
- [x] Update GitHub Actions: removed deployment step (CI only builds images)
- [x] Verify: Full GitOps flow working (git is source of truth)
- [x] Test: Self-healing (manual scale down auto-restored to 2 replicas)
- [x] Learn: ArgoCD sync, self-healing, git-based deployment, polling vs webhooks

### Phase 2B: MongoDB via GitOps

#### 2.4: MongoDB Helm Chart with Sealed Secrets
- [x] Create Helm chart for MongoDB (`helm/charts/mongodb/`)
- [x] Configure StatefulSet for MongoDB
- [x] Create PersistentVolumeClaim templates (5Gi storage)
- [x] Install Sealed Secrets controller in K3s cluster
- [x] Install `kubeseal` CLI tool locally
- [x] Create encrypted SealedSecret for MongoDB credentials
- [x] Configure headless ClusterIP Service for StatefulSet
- [x] Create ArgoCD Application manifest for MongoDB
- [x] Verify: Chart templates render correctly with encrypted secrets
- [x] Learn: StatefulSets vs Deployments, persistent storage, Sealed Secrets encryption
- [x] Security: Credentials encrypted and safe to commit to public repo

#### 2.5: Deploy MongoDB via ArgoCD
- [x] Apply ArgoCD Application manifest to cluster
- [x] Verify: MongoDB pod running, PVC bound, ArgoCD synced
- [x] Test: Connect to MongoDB from within cluster
- [x] Verify: Data persists across pod restarts
- [x] Learn: StatefulSet pod identity, volume attachment, ArgoCD management

#### 2.6: Test with Simple .NET API
- [ ] Create minimal .NET 9 API project
- [ ] Add MongoDB.Driver NuGet package
- [ ] Implement simple health endpoint
- [ ] Implement one CRUD endpoint (e.g., POST/GET a message)
- [ ] Containerize the API
- [ ] Create Helm chart for .NET API
- [ ] Create ArgoCD Application for API
- [ ] Test: Write data via API → verify in MongoDB
- [ ] Verify: End-to-end GitOps flow for all components
- [ ] Learn: .NET + MongoDB integration, multi-app ArgoCD management

## Current Status
**PHASE 2B IN PROGRESS: MongoDB Deployed via ArgoCD**

MongoDB successfully deployed and running:
- Pod `mongodb-0` running with 1/1 ready status
- PersistentVolumeClaim `mongodb-data-mongodb-0` bound with 5Gi storage
- ArgoCD Application showing Healthy & Synced status
- All components green in ArgoCD UI (StatefulSet, Service, Secret, PVC)
- Ready for .NET API integration testing

Next: Create simple .NET API to test MongoDB connectivity and CRUD operations

## Architecture After Phase 2

```
GitHub Repo (Source of Truth)
    ├── charts/healthcheck/     (Helm chart)
    ├── charts/mongodb/         (Helm chart)  
    ├── charts/dotnet-api/      (Helm chart)
    └── argocd/applications/    (ArgoCD app manifests)
           ↓
      ArgoCD (watches repo)
           ↓
    K3s Cluster Deployments
    ├── healthcheck (nginx)
    ├── mongodb (StatefulSet + PVC)
    └── dotnet-api (talks to MongoDB)
```

## Notes
- Helm charts should be reusable and configurable
- ArgoCD handles all deployments - GitHub Actions only builds images
- Keep .NET API simple - focus is on infrastructure, not features
- MongoDB credentials managed via Kubernetes Secrets
- Test each component independently before moving to next step

## Completed
### Helm Chart Implementation (2.1)
- ✓ Helm chart created with Chart.yaml, values.yaml, and templates/
- ✓ Deployment and Service templates use `.Chart.Name` and `.Values.*` for flexibility
- ✓ GitHub Actions workflow updated to use `helm upgrade --install`
- ✓ Workflow overrides image tag with commit SHA for reliable deployments
- ✓ Successfully deployed with 2 replicas
- ✓ Security fix: Added `log-public-key: false` to prevent SSH key exposure
- ✓ Validated with `helm lint` and `helm template`
- ✓ Old `k3s/nginx-health-check-page.yaml` approach replaced

### ArgoCD Installation (2.2)
- ✓ ArgoCD v3.19.0 installed in `argocd` namespace
- ✓ Caddy reverse proxy configured for `argocd.kube-snake.mymh.dev`
- ✓ DNS A record added and propagated
- ✓ ArgoCD UI accessible with HTTPS and security headers
- ✓ Initial admin credentials retrieved and secured
- ✓ Learned: Caddy can't resolve `.svc.cluster.local` DNS (used ClusterIP instead)

### GitOps Workflow Integration (2.3)
- ✓ ArgoCD Application manifest created in `argocd/applications/healthcheck.yaml`
- ✓ Application configured with automated sync policy (prune + selfHeal)
- ✓ Connected to GitHub repo: `https://github.com/mymh13/kube-snake.git`
- ✓ Targeting `helm/charts/healthcheck` on `main` branch
- ✓ ArgoCD successfully synced and deployed healthcheck
- ✓ Tested self-healing: manual scale down auto-restored to 2 replicas
- ✓ GitHub Actions workflow simplified: removed deployment job, only builds images
- ✓ Full separation achieved: CI (GitHub Actions) builds, CD (ArgoCD) deploys
- ✓ Learned: ArgoCD polls Git every 3 minutes, no webhooks needed

### MongoDB Helm Chart with Sealed Secrets (2.4)
- ✓ Created MongoDB Helm chart structure in `helm/charts/mongodb/`
- ✓ Installed Bitnami Sealed Secrets controller (v0.24.5) in K3s cluster
- ✓ Installed `kubeseal` CLI tool locally for encryption
- ✓ Created SealedSecret with encrypted MongoDB credentials (username, password, database)
- ✓ StatefulSet configured with:
  - MongoDB 7.0 official image
  - Environment variables pulled from SealedSecret
  - Volume mount at `/data/db` for data persistence
  - Resource limits: 500m CPU, 512Mi memory
- ✓ PersistentVolumeClaim template configured:
  - 5Gi storage using K3s `local-path` storage class
  - ReadWriteOnce access mode
  - Automatically created for each StatefulSet pod
- ✓ Headless Service created (clusterIP: None) for StatefulSet pod DNS
- ✓ ArgoCD Application manifest created in `argocd/applications/mongodb.yaml`
- ✓ Validated chart with `helm lint` and `helm template`
- ✓ Configured local kubectl to connect to K3s cluster from Windows machine
- ✓ Learned: 
  - Sealed Secrets encrypts with public key, only cluster's private key can decrypt
  - StatefulSets provide stable network identity (mongodb-0, mongodb-1, etc.)
  - VolumeClaimTemplates auto-create PVCs for each replica
  - Headless services provide direct pod DNS resolution
  - Encrypted secrets are cryptographically secure for public repos

**Key Security Achievement:** 
- Credentials encrypted before committing to Git
- Only K3s cluster can decrypt them
- Safe to store in public GitHub repository
- No plaintext secrets in version control

### MongoDB Deployment via ArgoCD (2.5)
- ✓ Applied ArgoCD Application manifest to K3s cluster
- ✓ ArgoCD detected and synced MongoDB Helm chart from Git
- ✓ Sealed Secrets controller decrypted credentials automatically
- ✓ StatefulSet created pod `mongodb-0` successfully
- ✓ PersistentVolumeClaim `mongodb-data-mongodb-0` bound to 5Gi storage volume
- ✓ Headless Service `mongodb-service` providing DNS resolution
- ✓ ArgoCD UI showing all resources as Healthy & Synced:
  - StatefulSet: `mongodb` (green)
  - Pod: `mongodb-0` (running, 1/1)
  - Service: `mongodb-service` (healthy)
  - SealedSecret: `mongodb-credentials` (created)
  - Secret: `mongodb-credentials` (decrypted)
  - PVC: `mongodb-data-mongodb-0` (bound)
- ✓ Verified: Pod running for 3+ minutes without restarts
- ✓ Learned:
  - ArgoCD Application manifests trigger immediate deployment
  - StatefulSet pods get stable identity (mongodb-0)
  - PVCs automatically created from volumeClaimTemplates
  - Sealed Secrets controller works seamlessly with ArgoCD sync
  - GitOps deployment complete in seconds after `kubectl apply`

**Key Achievement:**
- Full GitOps workflow operational for stateful workloads
- MongoDB deployed entirely through Git → ArgoCD → K3s
- No manual kubectl commands for deployment (only for ArgoCD app registration)
- Persistent storage working, data will survive pod restarts

## What We'll Learn
- Helm chart creation and templating
- ArgoCD installation and configuration
- GitOps workflow and automatic synchronization
- StatefulSets and persistent storage in Kubernetes
- Secrets management in K8s
- Multi-application orchestration with ArgoCD
- .NET containerization and MongoDB integration

## Success Criteria
Healthcheck deployed via ArgoCD + Helm (no more manual kubectl)  
ArgoCD UI accessible and showing all apps in sync  
MongoDB running with persistent storage  
Simple .NET API can write/read from MongoDB  
Git push triggers automatic deployment via ArgoCD  
All infrastructure as code in Git repository