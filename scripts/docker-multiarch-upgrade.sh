#!/bin/bash

# 多架构Docker容器升级脚本
# 用于更新服务器上的多架构Docker容器

# 彩色输出
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# 定义一些变量
IMAGE_NAME="liaoshiquan/midjourney-proxy"
CONTAINER_NAME="stone-mjopen"
EXTERNAL_PORT=8036
INTERNAL_PORT=8080

# 打印信息
echo -e "${BLUE}开始更新 ${YELLOW}${CONTAINER_NAME}${BLUE} 容器...${NC}"

# 验证Docker是否安装
if ! command -v docker &> /dev/null
then
    echo -e "${RED}Docker 未安装，请先安装 Docker。${NC}"
    exit 1
fi

# 拉取最新镜像
echo -e "${BLUE}拉取最新的镜像 ${YELLOW}${IMAGE_NAME}${BLUE}...${NC}"
docker pull ${IMAGE_NAME}
if [ $? -ne 0 ]; then
    echo -e "${RED}拉取镜像失败，请检查网络连接或镜像地址是否正确。${NC}"
    exit 1
fi

# 停止并移除现有容器
if [ "$(docker ps -q -f name=${CONTAINER_NAME})" ]; then
    echo -e "${BLUE}停止现有的容器 ${YELLOW}${CONTAINER_NAME}${BLUE}...${NC}"
    docker stop ${CONTAINER_NAME}
    if [ $? -ne 0 ]; then
        echo -e "${RED}停止容器失败，请手动检查。${NC}"
        exit 1
    fi
fi

if [ "$(docker ps -aq -f status=exited -f name=${CONTAINER_NAME})" ]; then
    echo -e "${BLUE}移除现有的容器 ${YELLOW}${CONTAINER_NAME}${BLUE}...${NC}"
    docker rm ${CONTAINER_NAME}
    if [ $? -ne 0 ]; then
        echo -e "${RED}移除容器失败，请手动检查。${NC}"
        exit 1
    fi
fi

# 运行新的容器
echo -e "${BLUE}启动新的容器 ${YELLOW}${CONTAINER_NAME}${BLUE}...${NC}"
docker run --name ${CONTAINER_NAME} -d --restart=always \
 -p ${EXTERNAL_PORT}:${INTERNAL_PORT} --user root \
 -v /root/mjopen/logs:/app/logs:rw \
 -v /root/mjopen/data:/app/data:rw \
 -v /root/mjopen/attachments:/app/wwwroot/attachments:rw \
 -v /root/mjopen/ephemeral-attachments:/app/wwwroot/ephemeral-attachments:rw \
 -e TZ=Asia/Shanghai \
 -v /etc/localtime:/etc/localtime:ro \
 -v /etc/timezone:/etc/timezone:ro \
 ${IMAGE_NAME}
if [ $? -ne 0 ]; then
    echo -e "${RED}启动新的容器失败，请手动检查。${NC}"
    exit 1
fi

# 获取本机IP
public_ip=$(curl -s ifconfig.me 2>/dev/null || echo "无法获取")
private_ip=$(hostname -I 2>/dev/null | awk '{print $1}' || echo "无法获取")

echo -e "${GREEN}容器 ${YELLOW}${CONTAINER_NAME}${GREEN} 更新并启动成功！${NC}"
echo -e "${GREEN}访问地址:${NC}"
echo -e "  内网: ${BLUE}http://${private_ip}:${EXTERNAL_PORT}${NC}"
echo -e "  外网: ${BLUE}http://${public_ip}:${EXTERNAL_PORT}${NC}" 