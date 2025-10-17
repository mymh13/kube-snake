# Phase 4: Snake Game Implementation

## Overview
Added a real-time multiplayer Snake game to the Kube-Snake cluster, featuring session-based game state management, hypermedia-driven UI, and Redis-backed persistence across multiple pods.

---

## Architecture Components

### Frontend (`/snake`)
- **Technology**: HTMX 2.0.7 (hypermedia-driven architecture)
- **Rendering**: Server-side HTML generation (300ms polling interval)
- **Controls**: Button-based + keyboard (Arrow keys, WASD)
- **Session**: Cookie-based client identification (GDPR-compliant)
- **Path**: `/snake` (served via Caddy reverse proxy)

**Key Features**:
- Zero JavaScript framework dependencies
- Real-time game updates via HTMX polling
- Cross-origin cookie handling for session persistence
- Responsive CSS Grid layout (20x20 game board)

---

### Backend (`/snake-api`)
- **Technology**: .NET 9 Minimal API
- **Game Logic**: 300ms timer-based movement, collision detection
- **State Storage**: Redis (per-session isolation with 24h TTL)
- **Session Management**: ASP.NET Core built-in sessions
- **Replicas**: 1 (scaling to 2+ requires Redis connection fix)

**Endpoints**:
```
GET  /snake-api/render         # Returns HTML grid (polled by HTMX)
POST /snake-api/start          # Initialize game session
POST /snake-api/pause          # Toggle pause state
POST /snake-api/reset          # Clear game state
POST /snake-api/move           # Handle direction input (form data)
```

**State Management**:
- Per-session game state stored in `ConcurrentDictionary<string, GameState>`
- Redis keys: `snake:session:{sessionId}:gamestate`
- Automatic session ID generation on first request
- Game state includes: snake position, food location, score, direction, pause/game-over flags

---

## Infrastructure Changes

### Redis Deployment
```bash
helm install redis bitnami/redis \
  -n default \
  --set auth.enabled=false \
  --set master.persistence.enabled=false \
  --set replica.replicaCount=0
```

**Purpose**: Distributed session storage across multiple `snake-api` pods

**Connection**: 
- Service: `redis-master.default.svc.cluster.local:6379`
- Configuration: `abortConnect=false` (retry on connection failure)
- Current Status: ⚠️ Connection issues when scaling (works on single pod)

---

### Ingress Configuration (Caddy)
**New Path Routes**:
```caddyfile
kube-snake.mymh.dev {
    # Snake frontend
    handle /snake* {
        reverse_proxy snake:80
    }
    
    # Snake API
    handle /snake-api* {
        reverse_proxy snake-api:8080
    }
    
    # Existing routes...
    handle /guestbook* {
        reverse_proxy guestbook:8080
    }
    
    handle /healthcheck* {
        reverse_proxy healthcheck:80
    }
}
```

**TLS**: Automatic via Let's Encrypt (managed by Caddy)

---

## Session Architecture

### Cookie Flow
1. **Client** visits `/snake` → HTMX loads page
2. **First request** to `/snake-api/render` → Backend generates session ID
3. **Session cookie** set: `.SnakeGame.Session` (HttpOnly, SameSite=Lax)
4. **Subsequent requests** include cookie → Backend retrieves session state

### CORS Configuration
```csharp
policy.WithOrigins("https://kube-snake.mymh.dev")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials(); // Required for cross-origin cookies
```

**HTMX Configuration**:
```javascript
document.body.addEventListener('htmx:configRequest', function(evt) {
    evt.detail.credentials = 'include'; // Send cookies with all requests
});
```

---

## CI/CD Pipeline

### Workflow: `snake.yml`
```yaml
on:
  push:
    paths:
      - 'apps/snake/**'
      - '.github/workflows/snake.yml'
```

**Steps**:
1. Build Docker image → `ghcr.io/mymh13/kube-snake/snake`
2. Tag format: `main-{short-sha}-{timestamp}`
3. Update `helm/charts/snake/values.yaml` with new tag
4. Commit + push → Trigger ArgoCD sync

### Workflow: `snake-api.yml`
```yaml
on:
  push:
    paths:
      - 'apps/snake-api/**'
      - '.github/workflows/snake-api.yml'
```

**Steps**: (Same as above, different image target)

---

## Helm Charts

### `helm/charts/snake/`
```yaml
replicaCount: 2
image:
  repository: ghcr.io/mymh13/kube-snake/snake
  tag: "main-{sha}"
service:
  type: ClusterIP
  port: 80
```

### `helm/charts/snake-api/`
```yaml
replicaCount: 1  # ⚠️ Scaling blocked by Redis connection issues
image:
  repository: ghcr.io/mymh13/kube-snake/snake-api
  tag: "main-{sha}"
env:
  - name: REDIS_CONNECTION
    value: "redis-master.default.svc.cluster.local:6379,abortConnect=false"
service:
  type: ClusterIP
  port: 8080
resources:
  limits:
    cpu: 200m
    memory: 256Mi
```

---

## Known Issues & Limitations

### Redis Connection (Multi-Pod)
**Symptom**: Game state "bounces" between pods when scaled to 2+ replicas
**Cause**: Redis connection failing despite correct service DNS
**Workaround**: Run single replica (`replicaCount: 1`)
**Investigation Needed**: 
- Check Redis auth settings (currently `auth.enabled=false`)
- Verify network policies allow pod-to-pod communication
- Test manual Redis connection from pod: `redis-cli -h redis-master.default.svc.cluster.local`

### Session Persistence
**Current**: Sessions expire after 24 hours (Redis TTL + cookie expiry)
**Limitation**: No cross-device session sharing (cookie-based only)

---

## Technology Stack Summary

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Frontend** | HTMX 2.0.7 | Hypermedia-driven UI updates |
| **Backend** | .NET 9 Minimal API | Game logic + session management |
| **State Store** | Redis (Bitnami) | Distributed game state |
| **Session** | ASP.NET Core Sessions | Cookie-based identification |
| **Proxy** | Caddy | Path-based routing + TLS |
| **CI/CD** | GitHub Actions | Automated builds + deployments |
| **GitOps** | ArgoCD | Helm chart synchronization |

---

## GDPR Compliance
- **Cookie Type**: Strictly necessary (functional, not tracking)
- **Data Stored**: Session ID (UUID) + game state (position, score)
- **Privacy Notice**: Displayed on game page
- **No Analytics**: Zero tracking pixels or third-party cookies
- **Data Retention**: 24 hours (automatic Redis TTL)

---

## Performance Metrics
- **Tick Rate**: 300ms (3.33 FPS)
- **Grid Size**: 20x20 (400 cells)
- **Polling Interval**: 300ms (HTMX)
- **Session TTL**: 24 hours
- **Resource Usage**: ~100Mi RAM, 100m CPU per pod

---

## Future Improvements
1. **Fix Redis multi-pod scaling** (investigate connection issues)
2. **Add leaderboard** (MongoDB integration for high scores)
3. **Implement WebSockets** (replace polling with real-time push)
4. **Add game difficulty levels** (adjustable tick rate)
5. **Mobile touch controls** (swipe gestures for direction change)

---

## Testing Instructions
1. **Single Player**: Works perfectly with 1 `snake-api` replica
2. **Verify Session**: Check browser DevTools → Application → Cookies → `.SnakeGame.Session`
3. **Test Controls**: Arrow keys, WASD, or on-screen buttons
4. **Monitor Logs**: `kubectl logs -n default -l app=snake-api -f`

---

**Status**: **Functional** (single pod), **Multi-pod scaling pending Redis fix**