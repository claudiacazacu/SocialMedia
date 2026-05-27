# Stage 1: Build Angular cu Node 20
FROM node:20-alpine AS build
WORKDIR /src
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build -- --configuration production

# Stage 2: Serve static cu nginx
FROM nginx:1.27-alpine
# Angular 17+: dist/<project-name>/browser/
COPY --from=build /src/dist/frontend/browser/ /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
