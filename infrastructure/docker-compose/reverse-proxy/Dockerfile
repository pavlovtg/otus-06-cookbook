FROM nginx:1.27-alpine AS builder

RUN apk add --no-cache \
    build-base \
    pcre-dev \
    zlib-dev \
    openssl-dev \
    git \
    wget

ARG NGINX_VERSION=1.27.5

RUN wget -q "http://nginx.org/download/nginx-${NGINX_VERSION}.tar.gz" \
    && tar -xzf "nginx-${NGINX_VERSION}.tar.gz"

RUN git clone --depth=1 https://github.com/nginx-modules/ngx_cache_purge.git

RUN cd "nginx-${NGINX_VERSION}" \
    && ./configure \
        --with-compat \
        --add-dynamic-module=../ngx_cache_purge \
    && make modules \
    && cp objs/ngx_http_cache_purge_module.so /tmp/ngx_http_cache_purge_module.so

FROM nginx:1.27-alpine

COPY --from=builder /tmp/ngx_http_cache_purge_module.so /usr/lib/nginx/modules/

RUN sed -i '1s|^|load_module /usr/lib/nginx/modules/ngx_http_cache_purge_module.so;\n|' /etc/nginx/nginx.conf
