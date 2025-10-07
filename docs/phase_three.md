# Phase 3: Guestbook Application (.NET + MongoDB + HTMX)

## Goal
Build a permanent, production-ready guestbook feature that:
- Demonstrates MongoDB integration with .NET
- Serves as backup deliverable if Snake game isn't completed
- Can be embedded in multiple pages (healthcheck now, Snake game later)
- Uses modern, minimal tech stack (HTMX, no heavy JavaScript frameworks)

## Philosophy
- **Permanent feature, not throwaway code** - this is a real application component
- **Reusable component** - designed to be embedded anywhere
- **HTMX-first** - minimal JavaScript, hypermedia-driven interactions
- **Secure by default** - admin authentication via Sealed Secrets
- **GitOps deployment** - containerized, Helm chart, ArgoCD managed

## Architecture

```
User Browser
    ↓ (HTMX from healthcheck/Snake game HTML)
Healthcheck/Snake Game Pod
    ↓ (HTTP to http://guestbook-service/api/*)
Guestbook API Pod (.NET Minimal API)
    ↓ (MongoDB.Driver)
MongoDB StatefulSet
    ↓ (PersistentVolumeClaim)
VM Storage (5Gi)
```

**Key: Guestbook is API-only - no standalone page, accessed via K8s service discovery**

## Steps

### Phase 3.1: .NET Guestbook API COMPLETED
- [x] Create .NET 9 Minimal API project (`apps/guestbook/`) with own .sln
- [x] Add MongoDB.Driver NuGet package
- [x] Configure MongoDB connection (read from environment variables)
- [x] Create `Message` model (Id, Text, CreatedAt, CreatedBy)
- [x] Implement endpoints (API-only, returns HTML partials for HTMX):
  - `GET /api/messages` - Get last 10 messages as HTML partial (public)
  - `POST /api/login` - Admin login (returns session cookie + HTML swap)
  - `POST /api/messages` - Create message (admin only, returns updated list)
  - `DELETE /api/messages/{id}` - Delete message (admin only, returns updated list)
  - `POST /api/logout` - Clear session
- [x] Implement cookie-based authentication
- [x] Add admin credentials from environment variables
- [x] Test locally: `dotnet run` and verify endpoints work via port-forward
- [x] **Troubleshooting:** Resolved port-forward conflicts (see [Lessons Learned: Port Forwarding](#lessons-learned-port-forwarding))
- [x] Learn: .NET Minimal APIs, MongoDB.Driver, cookie auth, returning HTML from APIs

### Phase 3.2: HTMX Frontend Component
- [ ] Create `index.html` with embedded HTMX
- [ ] Design layout:
  - Public section: Display last 10 messages (read-only)
  - Admin section: Login form OR admin panel (conditional)
- [ ] Implement HTMX interactions:
  - `hx-get="/api/messages"` - Load messages into `#messages-list`
  - `hx-post="/api/login"` - Submit login, swap to admin panel
  - `hx-post="/api/messages"` - Post new message, refresh list
  - `hx-delete="/api/messages/{id}"` - Delete message, refresh list
- [ ] Add minimal CSS styling (keep it simple, professional)
- [ ] No JavaScript required (pure HTMX + HTML)
- [ ] Test: All interactions work without page reloads
- [ ] Learn: HTMX attributes, hypermedia-driven UI, server-side rendering

### Phase 3.3: Containerize & Deploy via GitOps
- [x] Create Dockerfile for .NET guestbook
- [x] Build multi-stage image (sdk → runtime)
- [x] Test Docker build locally
- [ ] Create GitHub Actions workflow (`build-guestbook.yaml`):
  - Trigger on `apps/guestbook/**` changes
  - Build image with commit SHA tag
  - Push to GHCR (`ghcr.io/mymh13/guestbook:latest` and `:sha`)
- [x] Create Helm chart (`helm/charts/guestbook/`)
  - Deployment with environment variables for MongoDB connection
  - Service (ClusterIP, port 80)
  - Secrets references for credentials
- [x] Create ArgoCD Application (`argocd/applications/guestbook.yaml`)
- [ ] Apply to cluster: `kubectl apply -f argocd/applications/guestbook.yaml`
- [ ] Verify: ArgoCD syncs, pod running, can access guestbook
- [ ] Learn: Multi-app GitOps, environment configuration, service discovery

### Phase 3.4: Integrate with Healthcheck Page
- [x] Update healthcheck `index.html`:
  - Keep existing system info section (top)
  - Add guestbook section below with HTMX calls to `http://guestbook-service/api/*`
- [ ] Test: Healthcheck page shows system info + guestbook
- [ ] Verify: Admin login works, messages persist in MongoDB
- [ ] Test: Pod restart → data survives (PersistentVolume working)
- [ ] Verify: Public access at `https://kube-snake.mymh.dev` shows both components
- [ ] Learn: K8s service discovery, HTML composition, inter-pod communication

## Current Status
**PHASE 3.3: IN PROGRESS** - Helm chart created, awaiting GitHub Actions workflow
**PHASE 3.4: HTMX integration added to healthcheck** - awaiting cluster deployment to test

MongoDB integration working locally via port-forward. Ready to build HTMX frontend.

## Tech Stack
- **Backend:** .NET 9 Minimal API
- **Database:** MongoDB (deployed in Phase 2)
- **Frontend:** HTML + HTMX + minimal CSS
- **Auth:** Cookie-based sessions, admin credentials from environment variables
- **Container:** Docker multi-stage build (pending)
- **Deployment:** Helm + ArgoCD (GitOps) (pending)
- **Security:** Kubernetes Secrets (manual), Sealed Secrets (planned)

## MongoDB Schema

### Messages Collection
```json
{
  "_id": ObjectId,
  "text": "Hello world!",
  "createdAt": ISODate("2025-10-07T10:00:00Z"),
  "createdBy": "admin"
}
```

### Indexes
- `createdAt` descending (for "last 10 messages" query)

## API Endpoints (API-only, returns HTML partials)

| Method | Path | Auth | Returns | Description |
|--------|------|------|---------|-------------|
| GET | `/api/messages` | Public | HTML | Last 10 messages as HTML list |
| POST | `/api/login` | Public | HTML | Admin panel HTML (on success) |
| POST | `/api/messages` | Admin | HTML | Updated message list |
| DELETE | `/api/messages/{id}` | Admin | HTML | Updated message list |
| POST | `/api/logout` | Admin | HTML | Login form HTML |

## HTMX Interactions

```html
<!-- Public message list -->
<div id="messages-list" 
     hx-get="/api/messages" 
     hx-trigger="load, every 30s">
  <!-- Messages injected here -->
</div>

<!-- Admin login form -->
<form hx-post="/api/login" 
      hx-target="#admin-section" 
      hx-swap="outerHTML">
  <input name="username" />
  <input type="password" name="password" />
  <button>Login</button>
</form>

<!-- Admin panel (shown after login) -->
<div id="admin-section">
  <form hx-post="/api/messages" 
        hx-target="#messages-list" 
        hx-swap="innerHTML">
    <textarea name="text" maxlength="100"></textarea>
    <button>Post</button>
  </form>
</div>
```

## Security Considerations
- Admin credentials stored in Kubernetes Secrets (gitignored)
- MongoDB only accessible within cluster (ClusterIP service)
- Cookie-based auth with HttpOnly flag
- Input validation: max message length 100 chars
- No public write access without authentication
- Sealed Secrets planned for production deployment
- CSRF protection: Not needed (HTMX handles this with headers)

## Success Criteria
- [x] .NET API connects to MongoDB successfully
- [ ] Admin can login with credentials from environment variables
- [ ] Admin can post messages (stored in MongoDB)
- [ ] Admin can delete messages
- [x] Public users can view messages (read-only) - endpoint returns `<ul></ul>`
- [ ] Messages persist across pod restarts
- [ ] All interactions work via HTMX (no page reloads)
- [ ] Deployed via ArgoCD (GitOps)
- [ ] Accessible from healthcheck via K8s service: `http://guestbook-service/api/messages`

## Completed
- **Phase 3.1:** .NET Guestbook API with MongoDB integration
  - MongoDbService implemented with environment-based configuration
  - Message model created with CRUD endpoints
  - Authentication endpoints (login/logout) implemented
  - Local testing successful via port-forward (port 27018)
  - Kubernetes Secrets created (gitignored for security)
  - Secret templates committed for documentation

## What We'll Learn
- .NET 9 Minimal APIs 
- MongoDB.Driver for .NET 
- HTMX for hypermedia-driven UIs (upcoming)
- Cookie-based authentication 
- Multi-container application deployment (upcoming)
- Service-to-service communication in K8s (upcoming)
- Embedding components in existing pages (upcoming)
- State persistence with StatefulSets 
- Port-forward troubleshooting and conflict resolution 

## Notes
- Keep guestbook simple and focused - it's a component, not the main feature
- Design for reusability - should work embedded anywhere
- HTMX eliminates need for complex JavaScript framework
- Admin-only posting prevents spam without complex rate limiting
- Guestbook serves as backup deliverable if Snake game isn't finished

---

## Lessons Learned: Port Forwarding

### The Problem: Silent Port Conflicts
During local development testing, we encountered authentication failures where `mongosh` could successfully authenticate to MongoDB using credentials `user:password`, but the .NET MongoDB.Driver consistently failed with `MongoAuthenticationException: Unable to authenticate using sasl protocol mechanism SCRAM-SHA-1`. Initially, we suspected connection string parsing issues, URL encoding problems, or SCRAM mechanism mismatches between the MongoDB server and the .NET driver. After extensive troubleshooting including creating multiple test users, trying different authentication mechanisms (SCRAM-SHA-1 vs SCRAM-SHA-256), and using `MongoClientSettings` instead of connection strings, the root cause was discovered: MongoDB's default port 27017 was already bound locally by either a local MongoDB instance or a stale `kubectl port-forward` process.

### The Solution: Alternate Port Binding
The fix was to use an alternate local port for the port-forward tunnel: `kubectl port-forward svc/mongodb-service 27018:27017 -n default`. This ensured the tunnel actually forwarded traffic to the K3s MongoDB instance instead of routing to the conflicting local process. The .NET application's `.env` file was updated to use `mongodb://user:password@localhost:27018/?authSource=admin` for local development, while production deployments in the cluster use `mongodb://user:password@mongodb-service:27017/?authSource=admin` for direct service-to-service communication. Key takeaways: always verify `kubectl port-forward` shows `Forwarding from 127.0.0.1:XXXX` on