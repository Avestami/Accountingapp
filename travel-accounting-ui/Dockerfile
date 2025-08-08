# Build stage
FROM node:20-alpine as build-stage
WORKDIR /app
COPY travel-accounting-ui/package*.json ./
RUN npm install
COPY travel-accounting-ui/ .
RUN npm run build

# Production stage
FROM nginx:stable-alpine as production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY travel-accounting-ui/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]