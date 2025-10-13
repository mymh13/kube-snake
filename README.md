### Git repo: https://github.com/mymh13/kube-snake
  
### Author: https://github.com/mymh13
  
### Documentation in /docs/ -
architecture.md - architectural layout  
initialouttake.md - initial outtake of the project  
kube-snake-documenations.docx - raw Word file with timeline and summary of documenation  
kubeseal_sealed_secrets.md - kubeseal + sealed secrets documentation, came with install  
phase_one.md - documentation of phase one of the project  
phase_two.md - documentation of phase two  
phase_three.md - documentation of phase three  
  
### Basic project structure:
  
```bash
Kube-Snake                  # Project root
│
├───.github
│   └───workflows           # GitHub Actions CI/CD pipelines
│
├───apps                    # Web applications
│   ├───backend             # (Future: Snake game backend)
│   ├───frontend            # (Future: Snake game frontend)
│   ├───guestbook           # .NET 9 Minimal API guestbook app
│   │   ├───Endpoints       # API endpoints (Auth, Messages)
│   │   ├───Extensions      # Service configuration extensions
│   │   ├───Helpers         # HTML rendering, authentication helpers
│   │   ├───Models          # Data models (Message)
│   │   ├───Properties      # Project properties
│   │   └───Services        # MongoDB service
│   └───healthcheck         # System health monitoring page
│
├───argocd
│   └───applications        # ArgoCD Application manifests
│
├───docs                    # Project documentation (phases, architecture)
│
├───helm
│   └───charts              # Helm charts for deployments
│       ├───guestbook       # Guestbook application chart
│       │   └───templates   # K8s manifests (Deployment, Service, etc.)
│       ├───healthcheck     # Healthcheck application chart
│       │   └───templates
│       ├───mongodb         # MongoDB StatefulSet chart
│       │   └───templates
│       └───secrets         # SealedSecrets for credentials
│
└───k8s
    ├───cert-manager        # TLS certificate management
    ├───guestbook           # (Legacy/manual K8s manifests)
    └───traefik             # Ingress controller configuration
```

### Flowcharts
  
#### GitOps Deployment FLowchart
```bash
Developer Push
    ↓
GitHub Actions (build + tag image)
    ↓
Push to GHCR
    ↓
Update values.yaml (automated)
    ↓
ArgoCD polls Git (every 30 min)
    ↓
Detect change → Sync
    ↓
K3s pulls new image
    ↓
Rolling deployment
```
  
#### User Interaction FLowchart
```bash
User Browser (HTTPS)
    ↓
Caddy Reverse Proxy (VM:<port>)
    ↓
K3s Ingress
    ↓
Healthcheck Service → Healthcheck Pod
    ↓ (HTMX request to /guestbook/api/*)
Guestbook Service → Guestbook Pod (any replica)
    ↓
MongoDB (read/write messages)
    ↓
Redis (session validation)
    ↓
Response (HTML partial via HTMX)
```
  
#### Guestbook Admin Authentication FLowchart
```bash
Admin Login Form
    ↓
POST /guestbook/api/login
    ↓
Validate credentials (env vars from SealedSecret)
    ↓
Store session in Redis
    ↓
Set cookie (HttpOnly, Secure, SameSite=None)
    ↓
Return admin panel HTML
    ↓
Subsequent requests validate cookie → Redis lookup
    ↓
Any pod can serve authenticated requests
```
  