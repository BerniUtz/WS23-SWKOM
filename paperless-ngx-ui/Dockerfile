FROM node:lts-alpine AS build
WORKDIR /app

# Install build tools
RUN apk add python3 make g++

# Install dependencies
COPY package.json package-lock.json ./
RUN npm install

# Copy source code
COPY . .

# Build app
RUN npm run build

# Build runtime image
FROM nginx:alpine

# Copy built app
COPY --from=build /app/dist/paperless-ui /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf
