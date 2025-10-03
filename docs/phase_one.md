# The Masterplan

## Goal
Set up K3s infrastructure and CI/CD pipeline with simple healthcheck deployment.

## Steps

### 1. K3s Local Setup (Docker Desktop)
- [x] Install/verify K3s in Docker Desktop
- [x] Test basic kubectl commands
- [x] Deploy simple nginx healthcheck pod
- [x] Verify pod is running and accessible

### 2. Infrastructure Setup
- [ ] Prepare Cloud VM environment
- [ ] Install K3s on VM
- [ ] Configure networking and firewall
- [ ] Test remote kubectl access

### 3. CI/CD Pipeline
- [ ] Create GitHub Actions workflow
- [ ] Build and push container to GHCR
- [ ] Deploy to K3s cluster
- [ ] Verify automated deployment works

## Current Status
**Step 2.1**: Preparing Cloud VM for K3s installation

## Notes
- Keep it minimal - nginx healthcheck only
- Verify each step before moving forward
- Document any issues encountered

## Completed
- ✓ Local K3s cluster verified working
- ✓ Basic nginx deployment and service created (`k3s/nginx-health-check-page.yaml`)
- ✓ Internal cluster networking confirmed working 