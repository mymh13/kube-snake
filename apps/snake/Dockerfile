FROM caddy:2.7-alpine

# Copy Caddyfile
COPY Caddyfile /etc/caddy/Caddyfile

# Create snake directory and copy files there
RUN mkdir -p /usr/share/caddy/snake
COPY index.html /usr/share/caddy/snake/index.html
COPY styles.css /usr/share/caddy/snake/styles.css
COPY favicon.ico /usr/share/caddy/snake/favicon.ico

# Also copy to root for fallback
COPY index.html /usr/share/caddy/index.html
COPY styles.css /usr/share/caddy/styles.css
COPY favicon.ico /usr/share/caddy/favicon.ico

EXPOSE 80