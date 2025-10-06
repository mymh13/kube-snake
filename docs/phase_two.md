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
- [ ] Deploy ArgoCD to K3s cluster
- [ ] Access ArgoCD UI (port-forward or via Caddy subdomain)
- [ ] Configure initial admin access
- [ ] Connect ArgoCD to GitHub repository
- [ ] Verify: ArgoCD dashboard accessible and repo connected
- [ ] Learn: ArgoCD architecture, sync mechanisms, GitOps workflow

#### 2.3: ArgoCD-Managed Healthcheck
- [ ] Create ArgoCD Application manifest for healthcheck
- [ ] Point ArgoCD to healthcheck Helm chart in repo
- [ ] Configure auto-sync policy
- [ ] Test: Git push → ArgoCD detects change → auto-sync → K3s deploy
- [ ] Update GitHub Actions: remove `kubectl` deploy step (CI only builds images)
- [ ] Verify: Full GitOps flow working (git is source of truth)
- [ ] Learn: ArgoCD sync, self-healing, git-based deployment

### Phase 2B: MongoDB via GitOps

#### 2.4: MongoDB Helm Chart
- [ ] Create Helm chart for MongoDB (`charts/mongodb/`)
- [ ] Configure StatefulSet for MongoDB
- [ ] Create PersistentVolumeClaim templates
- [ ] Create Kubernetes Secret for credentials (in values)
- [ ] Configure ClusterIP Service for internal access
- [ ] Create ArgoCD Application for MongoDB
- [ ] Deploy via ArgoCD sync
- [ ] Verify: MongoDB pod running, PVC bound, ArgoCD synced
- [ ] Learn: StatefulSets, persistent storage in K3s, secrets management

#### 2.5: Test with Simple .NET API
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
**PHASE 2.1 COMPLETE! ✅**

Successfully converted healthcheck deployment to Helm chart with 2 replicas. CI/CD pipeline now uses Helm for deployments with SHA-based image tagging. Ready to install ArgoCD.

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
