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

RUN git clone --depth=1 https://github.com/FRiCKLE/ngx_cache_purge.git

RUN cd "nginx-${NGINX_VERSION}" \
    && ./configure \
        --with-compat \
        --add-dynamic-module=../ngx_cache_purge \
    && make modules

FROM nginx:1.27-alpine

COPY --from=builder /nginx-*/objs/ngx_http_cache_purge_module.so /usr/lib/nginx/modules/

RUN echo 'load_module /usr/lib/nginx/modules/ngx_http_cache_purge_module.so;' \
    > /etc/nginx/modules/ngx_http_cache_purge_module.conf
