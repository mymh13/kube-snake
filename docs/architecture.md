# Kube-Snake Architecture

## Overview
Simple microservices app with K8s, .NET + HTMX frontend, MongoDB storage.

## Components
- **k8s/**: Raw Kubernetes YAML files
- **helm/**: Helm charts for deployment
- **apps/**: Application code (will contain .NET app later)
- **argocd/**: ArgoCD configuration
- **.github/workflows/**: GitHub Actions CI/CD

## Deployment Strategy
1. Local development: Docker Desktop K8s
2. Production: Azure AKS + Hetzner MongoDB

## Storage
- Built-in K8s storage for stateful sets
- External MongoDB on Hetzner VM