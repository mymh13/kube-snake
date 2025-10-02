# The Masterplan

## Goal
Set up K3s infrastructure and CI/CD pipeline with simple healthcheck deployment.

## Steps

### 1. K3s Local Setup (Docker Desktop)
- [ ] Install/verify K3s in Docker Desktop
- [ ] Test basic kubectl commands
- [ ] Deploy simple nginx healthcheck pod
- [ ] Verify pod is running and accessible

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
**Step 1.1**: Setting up K3s locally in Docker Desktop

## Notes
- Keep it minimal - nginx healthcheck only
- Verify each step before moving forward
- Document any issues encounteredlan
1. 