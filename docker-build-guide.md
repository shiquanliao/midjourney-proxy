# Midjourney Proxy Docker 构建与部署指南

本文档提供了在 macOS 环境下构建和部署 Midjourney Proxy Docker 镜像的详细步骤。

## 前提条件

- 已安装 Docker Desktop
- Docker 守护进程正在运行
- 已有 Docker Hub 账号

## 构建和部署流程

### 1. 确认 Docker 正在运行

```bash
docker info | head -5
```

如果显示 Docker 信息而非错误，则表示 Docker 守护进程正在运行。

### 2. 从 Dockerfile 构建镜像

在项目根目录（含有 src 文件夹的目录）执行：

```bash
docker build -t midjourney-proxy -f src/Midjourney.API/Dockerfile .
```

构建过程中，Docker 会：
- 使用 .NET SDK 镜像作为构建环境
- 复制项目文件
- 还原依赖包
- 编译并发布应用
- 创建最终的运行时镜像

### 3. 查看构建的镜像

```bash
docker images | grep midjourney
```

### 4. 为镜像添加 Docker Hub 标签

```bash
docker tag midjourney-proxy:latest [你的用户名]/midjourney-proxy:latest
```

例如：
```bash
docker tag midjourney-proxy:latest liaoshiquan/midjourney-proxy:latest
```

### 5. 推送镜像到 Docker Hub

首先确保已登录 Docker Hub：
```bash
docker login
```

然后推送镜像：
```bash
docker push [你的用户名]/midjourney-proxy:latest
```

例如：
```bash
docker push liaoshiquan/midjourney-proxy:latest
```

### 6. 本地运行镜像（可选）

```bash
docker run -d -p 8086:8080 --name mjproxy [你的用户名]/midjourney-proxy:latest
```

例如：
```bash
docker run -d -p 8086:8080 --name mjproxy liaoshiquan/midjourney-proxy:latest
```

### 7. 验证容器是否正在运行

```bash
docker ps | grep mjproxy
```

## 在生产环境部署

在生产服务器上，只需执行以下命令拉取并运行最新的镜像：

```bash
docker pull [你的用户名]/midjourney-proxy:latest
docker run -d -p 8086:8080 --name mjproxy \
  -v /path/to/logs:/app/logs:rw \
  -v /path/to/data:/app/data:rw \
  -v /path/to/attachments:/app/wwwroot/attachments:rw \
  -v /path/to/ephemeral-attachments:/app/wwwroot/ephemeral-attachments:rw \
  -e TZ=Asia/Shanghai \
  [你的用户名]/midjourney-proxy:latest
```

## 通过 GitHub Actions 自动构建（可选）

项目已配置了 GitHub Actions 工作流程，当发布新的 GitHub Release 时，会自动构建并推送 Docker 镜像。工作流程配置位于 `.github/workflows/docker-hub.yml`。

## 使用脚本升级（服务器环境）

项目提供了一个升级脚本 `scripts/docker-upgrade.sh`，可以在服务器上使用该脚本自动拉取最新镜像并部署。 