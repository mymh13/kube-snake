FROM caddy:2.7-alpine

# Copy static files
COPY index.html /usr/share/caddy/index.html
COPY styles.css /usr/share/caddy/styles.css
COPY favicon.ico /usr/share/caddy/favicon.ico

# Caddy will serve files from /usr/share/caddy on port 80
EXPOSE 80