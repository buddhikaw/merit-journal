# NOTE: This Dockerfile is not needed as per requirements.
# The frontend will be deployed to AWS CloudFront, not as a Docker container.
# This file is kept for reference only and can be deleted.

# Build stage
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . ./
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
