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
- [x] Prepare Cloud VM environment
- [x] Install K3s on VM
- [x] Configure networking and firewall
- [x] Deploy nginx healthcheck to remote cluster
- [x] Configure Caddy reverse proxy
- [x] Verify DNS propagation and external access

### 3. CI/CD Pipeline
- [x] Create GitHub Actions workflow
- [x] Build and push container to GHCR
- [x] Configure GitHub Secrets
- [ ] Deploy to K3s cluster
- [ ] Verify automated deployment works

## Current Status
**Step 3.4**: Ready to test CI/CD deployment

## Notes
- Keep it minimal - nginx healthcheck only
- Verify each step before moving forward
- Document any issues encountered
- DNS propagation can take time - be patient!

## Completed
### Local Development (Step 1)
- ✓ Local K3s cluster verified working
- ✓ Basic nginx deployment and service created (`k3s/nginx-health-check-page.yaml`)
- ✓ Internal cluster networking confirmed working

### Remote Infrastructure (Step 2)
- ✓ Hetzner VM updated and rebooted (Ubuntu 24.04 LTS)
- ✓ K3s installed on VM with Traefik disabled (`--disable=traefik`)
- ✓ kubectl configured for non-sudo access
- ✓ Nginx healthcheck deployed to remote K3s cluster
- ✓ Service verified running (ClusterIP: 10.43.39.44:80)
- ✓ Caddy reverse proxy configured for `kube-snake.mymh.dev`
- ✓ DNS A record created at Loopia → 37.27.40.61
- ✓ DNS propagated successfully
- ✓ SSL certificate auto-provisioned by Caddy
- ✓ External access verified at https://kube-snake.mymh.dev

### CI/CD Pipeline (Step 3)
- ✓ Custom healthcheck Docker image created (`apps/healthcheck/`)
- ✓ Dockerfile and index.html configured
- ✓ GitHub Actions workflow created (`.github/workflows/deploy-healthcheck.yml`)
- ✓ K3s deployment updated to use custom image from GHCR
- ✓ GitHub Secrets configured (SSH_HOST, SSH_USER, SSH_PRIVATE_KEY)
- ⏳ Ready to test automated deployment 