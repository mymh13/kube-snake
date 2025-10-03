FROM nginx:alpine

# Copy custom HTML
FROM nginx:alpine

# Copy the healthcheck page and styles
COPY index.html /usr/share/nginx/html/
COPY styles.css /usr/share/nginx/html/

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s \
    CMD wget --quiet --tries=1 --spider http://localhost/ || exit 1
