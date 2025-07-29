#!/bin/bash

# 多架构Docker镜像构建脚本 - 简化版
# 用于构建支持linux/amd64和linux/arm64架构的Docker镜像并推送到Docker Hub

# 配置 - 根据需要修改
IMAGE_NAME="liaoshiquan/midjourney-proxy"
DOCKERFILE_PATH="src/Midjourney.API/Dockerfile"
PLATFORMS="linux/amd64,linux/arm64"

# 获取版本信息 (使用git标签或默认为latest)
VERSION=$(git describe --tags --abbrev=0 2>/dev/null || echo "latest")
[ -z "$VERSION" ] && VERSION="latest"

echo "开始构建多架构Docker镜像: ${IMAGE_NAME}"
echo "支持平台: ${PLATFORMS}"
echo "版本标签: ${VERSION} 和 latest"

# 检查并使用buildx构建器
BUILDER=$(docker buildx ls | grep multiarch || echo "")
if [ -z "$BUILDER" ]; then
  echo "创建新的buildx构建器: multiarch"
  docker buildx create --name multiarch --use
else
  echo "使用现有的multiarch构建器"
  docker buildx use multiarch
fi

# 启动构建器
docker buildx inspect --bootstrap

# 登录到Docker Hub (如果需要)
if [ -z "$(docker info | grep 'Username')" ]; then
  echo "正在登录Docker Hub..."
  docker login
fi

# 构建并推送多架构镜像
echo "开始构建并推送多架构Docker镜像 (需要几分钟时间)..."
docker buildx build --platform ${PLATFORMS} \
  -t ${IMAGE_NAME}:latest \
  -t ${IMAGE_NAME}:${VERSION} \
  -f ${DOCKERFILE_PATH} . --push

# 验证推送的镜像
echo "验证多架构镜像..."
docker buildx imagetools inspect ${IMAGE_NAME}:latest

echo "多架构Docker镜像已成功构建并推送!"
echo "镜像标签:"
echo "  - ${IMAGE_NAME}:latest"
echo "  - ${IMAGE_NAME}:${VERSION}" 