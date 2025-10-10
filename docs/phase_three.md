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
    ↓ (HTTP to http://guestbook-service/guestbook/api/*)
Guestbook API Pod (.NET Minimal API)
    ↓ (MongoDB.Driver)
    ↓ (Redis for session storage)
MongoDB StatefulSet + Redis StatefulSet
    ↓ (PVC: PersistentVolumeClaim)
VM Storage (5Gi)
```

**Key: Guestbook is API-only - no standalone page, accessed via K8s service discovery**

## Steps

### Phase 3.1: .NET Guestbook API
- [x] Create .NET 9 Minimal API project (`apps/guestbook/`) with own .sln
- [x] Add MongoDB.Driver NuGet package
- [x] Configure MongoDB connection (read from environment variables)
- [x] Create `Message` model (Id, Text, CreatedAt, CreatedBy)
- [x] Implement endpoints (API-only, returns HTML partials for HTMX):
  - `GET /guestbook/api/messages` - Get last 10 messages as HTML partial (public)
  - `POST /guestbook/api/login` - Admin login (returns session cookie + HTML swap)
  - `POST /guestbook/api/messages` - Create message (admin only, returns updated list)
  - `POST /guestbook/api/messages/delete` - Delete selected messages (admin only, returns updated list)
  - `POST /guestbook/api/logout` - Clear session
- [x] Implement cookie-based authentication
- [x] Add admin credentials from environment variables (via SealedSecret)
- [x] Test locally: `dotnet run` and verify endpoints work via port-forward
- [x] Troubleshooting: Resolved port-forward conflicts (MongoDB port 27017 → 27018)

### Phase 3.2: HTMX Frontend Component
- [x] Add guestbook section to healthcheck `index.html` with embedded HTMX
- [x] Design layout:
  - Public section: Display last 10 messages (read-only)
  - Admin section: Login form OR admin panel (conditional)
- [x] Implement HTMX interactions:
  - `hx-get="/guestbook/api/messages"` - Load messages into `#messages-list`
  - `hx-post="/guestbook/api/login"` - Submit login, swap to admin panel
  - `hx-post="/guestbook/api/messages"` - Post new message, refresh list, clear textarea
  - `hx-post="/guestbook/api/messages/delete"` - Delete selected messages, refresh list
- [x] Add minimal CSS styling (keep it simple, professional)
- [x] No JavaScript required (pure HTMX + HTML)
- [x] Test: All interactions work without page reloads

### Phase 3.3: Containerize & Deploy via GitOps
- [x] Create Dockerfile for .NET guestbook
- [x] Build multi-stage image (sdk → runtime)
- [x] Test Docker build locally
- [x] Create GitHub Actions workflow (`guestbook-ci.yml`):
  - Trigger on `apps/guestbook/**` changes
  - Build image with commit SHA tag (`main-<sha>`)
  - Push to GHCR (`ghcr.io/mymh13/kube-snake-guestbook:latest` and `:main-<sha>`)
  - Automatically update `helm/charts/guestbook/values.yaml` with new SHA tag
  - Commit and push updated values file to trigger ArgoCD sync
- [x] Create Helm chart (`helm/charts/guestbook/`)
  - Deployment with environment variables for MongoDB connection
  - Service (ClusterIP, port 8080)
  - SealedSecret for admin credentials
  - Health probes configured for `/guestbook/api/messages` endpoint
- [x] Create ArgoCD Application (`argocd/applications/guestbook.yaml`)
- [x] Apply to cluster: `kubectl apply -f argocd/applications/guestbook.yaml`
- [x] Verify: ArgoCD syncs, pod running, can access guestbook

### Phase 3.4: Multi-Replica Session Persistence
- [x] Configure path-based routing with `app.UsePathBase("/guestbook")`
- [x] Fix session cookie path to match base path (`/guestbook`)
- [x] Configure session with `SameSite=None` and `Secure=true` for HTTPS
- [x] Install Redis via Bitnami Helm chart for distributed session storage
- [x] Replace `AddDistributedMemoryCache()` with `AddStackExchangeRedisCache()`
- [x] Add `Microsoft.Extensions.Caching.StackExchangeRedis` NuGet package
- [x] Configure Redis connection: `guestbook-redis-master:6379`
- [x] Update session configuration to use Redis for multi-replica support
- [x] Ensure `app.UseSession()` is called before endpoint mapping in `Program.cs`
- [x] Test: Healthcheck page shows system info + guestbook
- [x] Verify: Admin login works, messages persist in MongoDB
- [x] Test: Pod restart → data survives (PersistentVolume working)
- [x] Verify: Session state persists across 2+ replicas
- [x] Test: All HTMX interactions (login, post, logout) work with multiple pods

### Phase 3.5: Automated Image Rollout
- [x] Update GitHub Actions workflow to tag images with commit SHA
- [x] Add workflow step to automatically update `helm/charts/guestbook/values.yaml` with new tag
- [x] Configure Personal Access Token (PAT) for CI Bot to commit and push changes
- [x] Fix Git push authentication to use PAT in remote URL
- [x] Test: Git push → image build → values.yaml update → ArgoCD sync → K3s deploy
- [x] Verify: No manual `kubectl rollout restart` needed
- [x] Verify: ArgoCD detects image tag changes and updates pods automatically

### Phase 3.6: Admin Message Management & UX Polish
- [x] Add bulk delete functionality for admin users
- [x] Implement checkboxes next to each message (disabled for public users, enabled for admins)
- [x] Wrap checkboxes in delete form for proper form submission
- [x] Add "Delete Selected" button in admin panel
- [x] Create `POST /guestbook/api/messages/delete` endpoint for bulk deletion
- [x] Update `RenderMessages` to conditionally render checkboxes based on admin status
- [x] Increase textarea size for better UX (400px width, resizable)
- [x] Fix timestamp display: use `DateTime.Now` instead of `DateTime.UtcNow`
- [x] Store actual username in session during login
- [x] Display username and timestamp correctly: `username (2025-10-10 12:46): message`
- [x] Auto-clear textarea after posting using HTMX `hx-on::after-request="this.reset()"`
- [x] Test: Admin can select and delete multiple messages at once
- [x] Verify: Public users see disabled checkboxes (visual consistency)
- [x] Verify: All UX improvements work seamlessly

## Current Status
**PHASE 3: COMPLETE **

**All objectives achieved:**
- ✅ Guestbook API deployed with MongoDB integration
- ✅ HTMX frontend embedded in healthcheck page
- ✅ Full GitOps workflow with automated image tagging and deployment
- ✅ Session persistence working with Redis across multiple replicas
- ✅ Admin authentication secured with SealedSecrets
- ✅ All HTMX interactions working reliably (login, post, delete, logout)
- ✅ Bulk message deletion for admins
- ✅ Proper timestamp and username display
- ✅ Smooth UX with auto-clearing forms and responsive textarea
- ✅ Public access at `https://kube-snake.mymh.dev/`

**🎉 Ready for Phase 4: Snake Game! 🎉**

## Tech Stack
- **Backend:** .NET 9 Minimal API
- **Database:** MongoDB (StatefulSet from Phase 2)
- **Session Storage:** Redis (distributed cache)
- **Frontend:** HTML + HTMX + minimal CSS
- **Auth:** Cookie-based sessions, SealedSecrets for credentials
- **Container:** Docker multi-stage build
- **Deployment:** Helm + ArgoCD (GitOps)
- **CI/CD:** GitHub Actions (builds, tags, updates manifests)

## MongoDB Schema

### Messages Collection
```json
{
  "_id": ObjectId,
  "text": "Hello world!",
  "createdAt": ISODate("2025-10-10T12:46:00Z"),
  "createdBy": "kingcantona"
}
```

### Indexes
- `createdAt` descending (for "last 10 messages" query)

## API Endpoints

| Method | Path | Auth | Returns | Description |
|--------|------|------|---------|-------------|
| GET | `/guestbook/api/messages` | Public | HTML | Last 10 messages as HTML list |
| POST | `/guestbook/api/login` | Public | HTML | Admin panel HTML (on success) |
| POST | `/guestbook/api/messages` | Admin | HTML | Updated message list |
| POST | `/guestbook/api/messages/delete` | Admin | HTML | Updated message list after bulk delete |
| POST | `/guestbook/api/logout` | Admin | HTML | Login form HTML |

## HTMX Interactions

```html
<!-- Public message list -->
<div id="messages-list" 
     hx-get="/guestbook/api/messages" 
     hx-trigger="load, every 30s">
  <!-- Messages injected here -->
</div>

<!-- Admin login form -->
<form hx-post="/guestbook/api/login" 
      hx-target="#admin-section" 
      hx-swap="outerHTML">
  <input name="username" />
  <input type="password" name="password" />
  <button>Login</button>
</form>

<!-- Admin panel with auto-clearing textarea -->
<div id="admin-section">
  <form hx-post="/guestbook/api/messages" 
        hx-target="#messages-list" 
        hx-swap="innerHTML"
        hx-on::after-request="this.reset()">
    <textarea name="text" maxlength="200" 
              style="width: 400px; height: 80px; resize: both;"></textarea>
    <button>Post</button>
  </form>
</div>

<!-- Delete form (wraps message list for admins) -->
<form hx-post="/guestbook/api/messages/delete" 
      hx-target="#messages-list" 
      hx-swap="innerHTML">
  <ul>
    <li>
      <input type="checkbox" name="messageIds" value="{id}">
      <strong>{username}</strong> ({date}): {text}
    </li>
  </ul>
  <button type="submit">Delete Selected</button>
</form>
```

## Security Implementation
- Admin credentials stored as SealedSecret (encrypted, safe for Git)
- MongoDB only accessible within cluster (ClusterIP service)
- Cookie-based auth with HttpOnly, SameSite=None, Secure=true
- Input validation: max message length 200 chars
- No public write access without authentication
- Redis for session storage (no auth for in-cluster communication)
- Delete functionality restricted to authenticated admins only

## Success Criteria
- [x] .NET API connects to MongoDB successfully
- [x] Admin can login with credentials from SealedSecret
- [x] Admin can post messages (stored in MongoDB)
- [x] Admin can delete multiple messages at once
- [x] Public users can view messages (read-only)
- [x] Messages persist across pod restarts
- [x] All interactions work via HTMX (no page reloads)
- [x] Deployed via ArgoCD (GitOps)
- [x] Accessible from healthcheck page at `https://kube-snake.mymh.dev/`
- [x] Session state persists across multiple replicas using Redis
- [x] Image updates deploy automatically via GitHub Actions + ArgoCD
- [x] Proper timestamp and username display
- [x] Smooth UX with auto-clearing forms

## Completed

### .NET Guestbook API (3.1)
- ✓ .NET 9 Minimal API project created with own .sln
- ✓ MongoDB.Driver NuGet package installed
- ✓ MongoDbService implemented with environment-based configuration
- ✓ Message model created with CRUD endpoints
- ✓ Authentication endpoints (login/logout) implemented
- ✓ Bulk delete endpoint implemented
- ✓ Username stored in session during login
- ✓ Timestamp using `DateTime.Now` for correct local time display
- ✓ Local testing successful via port-forward (port 27018 to avoid conflicts)
- ✓ SealedSecret created for admin credentials (gitignored plaintext)

### HTMX Frontend Component (3.2)
- ✓ Guestbook section added to healthcheck index.html
- ✓ HTMX interactions implemented for all endpoints
- ✓ Minimal CSS styling applied
- ✓ No JavaScript required (pure HTMX + HTML)
- ✓ Checkboxes added for message selection (admin-only enabled)
- ✓ Auto-clear textarea after posting using `hx-on::after-request="this.reset()"`
- ✓ Textarea sized appropriately (400px width, 80px height, resizable)

### Containerization & GitOps Deployment (3.3)
- ✓ Dockerfile created with multi-stage build
- ✓ GitHub Actions workflow created (`guestbook-ci.yml`)
- ✓ Workflow builds and pushes images with SHA tags
- ✓ Helm chart created with deployment, service, and secrets
- ✓ ArgoCD Application manifest created
- ✓ Health probes configured for `/guestbook/api/messages`

### Multi-Replica Session Persistence (3.4)
- ✓ Path-based routing configured with `app.UsePathBase("/guestbook")`
- ✓ Session cookie path set to `/guestbook`
- ✓ Cookie configured with SameSite=None and Secure=true for HTTPS
- ✓ Redis deployed via Bitnami Helm chart (`guestbook-redis`)
- ✓ `AddStackExchangeRedisCache` configured in .NET app
- ✓ `Microsoft.Extensions.Caching.StackExchangeRedis` NuGet package added
- ✓ Session storage migrated from in-memory to Redis
- ✓ `app.UseSession()` placed before endpoint mapping
- ✓ Tested with 2 replicas, session persists across pods

### Automated Image Rollout (3.5)
- ✓ GitHub Actions workflow updated to tag images with commit SHA
- ✓ Workflow step added to update `helm/charts/guestbook/values.yaml`
- ✓ Personal Access Token (PAT) configured for CI Bot authentication
- ✓ Git push URL updated to use PAT for write access
- ✓ Automated commit and push of updated values file
- ✓ ArgoCD detects changes and triggers sync automatically
- ✓ Manual `kubectl rollout restart` no longer needed

### Admin Message Management & UX Polish (3.6)
- ✓ Bulk delete functionality implemented
- ✓ Checkboxes added next to each message
- ✓ Public users see disabled checkboxes (visual consistency)
- ✓ Admin users can select multiple messages
- ✓ Delete form wraps message list for proper form submission
- ✓ "Delete Selected" button added
- ✓ `POST /guestbook/api/messages/delete` endpoint created
- ✓ Textarea size optimized (400px width, resizable)
- ✓ Timestamp display fixed using `DateTime.Now`
- ✓ Username display shows actual admin username from session
- ✓ Auto-clear textarea after posting with HTMX `hx-on::after-request`

## Notes
- Keep guestbook simple and focused - it's a component, not the main feature
- Design for reusability - should work embedded anywhere
- HTMX eliminates need for complex JavaScript framework
- Admin-only posting prevents spam without complex rate limiting
- Guestbook serves as backup deliverable if Snake game isn't finished
- Redis distributed cache is essential for session persistence with multiple replicas
- Automated image tagging ensures seamless GitOps deployments without manual intervention
- Bulk delete requires checkboxes to be inside form element for proper value submission
- HTMX `hx-on::after-request="this.reset()"` provides clean UX without JavaScript

## Key Learnings
- **Port-forward conflicts** can cause silent auth failures (use alternate ports like 27018)
- **Session state must be distributed** (Redis) for multi-replica deployments—in-memory cache does not work
- **Health probes must use correct path** including base path (`/guestbook/api/messages`)
- **Session middleware order matters** (`app.UseSession()` before endpoints)
- **Cookie configuration for HTTPS** requires SameSite=None and Secure=true
- **Image tag in `values.yaml` must match registry tag exactly** to avoid ImagePullBackOff
- **ArgoCD syncs when Git manifests change**, enabling automated rollouts
- **SHA-based tagging** allows precise version control and rollback
- **GitHub Actions requires PAT** with write permissions to push commits back to repository
- **Form elements must wrap input fields** for values to be submitted correctly
- **HTMX `hx-target` and `hx-swap`** enable partial page updates without JavaScript
- **HTMX `hx-on::after-request`** provides native form reset without DOM manipulation
- **DateTime.Now vs DateTime.UtcNow** - use Now for display in local time, UtcNow for storage/comparison
- **Store username in session** during login to display correctly in messages
- **HTMX is more efficient than JavaScript** - fewer CPU cycles, less memory, smaller payload, native browser APIs

---

## Phase 3 Complete!

**What We Built:**
- Production-ready guestbook with MongoDB persistence
- Secure admin authentication with SealedSecrets
- Multi-replica session management with Redis
- Fully automated GitOps deployment pipeline
- Clean HTMX-driven UI with zero JavaScript dependencies

**What We Learned:**
- Distributed systems require distributed state (Redis)
- GitOps automation eliminates manual deployments
- HTMX provides better UX with less code
- Security through simplicity and server-side control

**Next Up: Phase 4 - Snake Game! 🐍**

Let's build the main event!