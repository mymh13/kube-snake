FROM nginx:alpine

# Copy custom HTML
COPY index.html /usr/share/nginx/html/index.html

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s \
    CMD wget --quiet --tries=1 --spider http://localhost/ || exit 1
