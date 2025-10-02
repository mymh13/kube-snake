# Kube-Snake Architecture

## Overview
Kubernetes-driven .NET microservices system with HTMX frontend and MongoDB storage.
Focus: Cost-effective, incremental development with K3s.

## Repository Structure
- **k8s/**: Raw Kubernetes YAML files for K3s deployments
- **helm/**: Helm charts for deployment
- **apps/**: Application code (.NET API - Phase 2+)
- **argocd/**: ArgoCD configuration
- **.github/workflows/**: GitHub Actions CI/CD pipelines
- **docs/**: Architecture and design documentation

## Technology Stack

### Kubernetes
- **K3s**: Lightweight Kubernetes distribution (lower resource usage than full K8s)
- **Ingress**: Traefik (built-in with K3s) + Let's Encrypt for free TLS
- **Storage**: Local-path provisioner (built-in K3s) for basic persistence

### Application Components (Phase 2+)
- **Backend**: .NET API (containerized, multiple replicas)
- **Frontend**: HTMX web app hosted on Azure Static Web App (free tier)
- **State Management**: Cookie-based auth or external session store (Redis)
- **Database**: MongoDB in container with local volume on Cloud VM

### CI/CD Pipeline
- **GitHub Actions**: Build and push container images to GHCR
- **Deployment**: Helm/ArgoCD (or kubectl apply initially)
- **Secrets**: K8s Secrets for DB credentials, GitHub Secrets for deploy keys

## Deployment Strategy

### Phase 1 (Current Focus)
- Set up K3s infrastructure
- Configure CI/CD pipeline
- Deploy simple nginx healthcheck container
- Verify deployment and networking

### Phase 2+ (Future)
- Deploy .NET API backend
- Integrate MongoDB storage
- Add Redis for session management
- Deploy HTMX frontend to Azure Static Web App

## Infrastructure Layout

### Cloud VM (Existing/Pre-paid)
- K3s cluster running .NET API pods
- MongoDB container (external to K3s, persistent volume)
- Redis container (optional, for sessions/caching)
- RabbitMQ container (optional, for message queues)

### Azure
- Static Web App (free tier) hosting HTMX frontend
- Minimal Azure presence to keep costs down

### Traffic Flow
```
User → Azure Static Web App (HTMX) 
     → K3s Ingress (Traefik) 
     → .NET API Service (multiple replicas) 
     → MongoDB/Redis (separate containers on VM)
```

## Storage Strategy
- **Critical data**: MongoDB on VM with persistent volumes (outside K3s pods)
- **Temporary/dev volumes**: K3s local-path provisioner
- **Stateless pods**: All K3s pods remain stateless for easy scaling

## Monitoring & Logging (Future)
- Start simple: `kubectl logs`
- Optional: Prometheus + Grafana via Helm
- Avoid Azure Monitor/App Insights to minimize costs

## Security
- Network policies for pod-to-pod communication
- RBAC for K3s access control
- Kubernetes Secrets for sensitive data
- GitHub Secrets for CI/CD credentials