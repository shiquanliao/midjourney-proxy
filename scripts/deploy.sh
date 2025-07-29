#!/bin/bash

# Midjourney Proxy 完整部署脚本
# 该脚本会自动处理以下流程：
# 1. 从上游仓库同步最新代码
# 2. 构建多架构Docker镜像
# 3. 将脚本上传到服务器
# 4. 在服务器上执行升级

# 彩色输出
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# 配置 - 请根据需要修改
SERVER_USER="root"
SERVER_IP="your_server_ip"    # 请修改为你的服务器IP
SERVER_PATH="/www/self-mj-plus"  # 服务器上的路径

# 确认配置
echo -e "${YELLOW}即将开始部署流程，请确认以下配置:${NC}"
echo -e "服务器用户: ${BLUE}${SERVER_USER}${NC}"
echo -e "服务器IP:   ${BLUE}${SERVER_IP}${NC}"
echo -e "服务器路径: ${BLUE}${SERVER_PATH}${NC}"
echo ""
read -p "配置是否正确? [y/n]: " confirm
if [[ "$confirm" != "y" && "$confirm" != "Y" ]]; then
    echo -e "${RED}部署已取消。请编辑脚本更新配置后重试。${NC}"
    exit 1
fi

# 开始部署流程
echo -e "${BLUE}开始部署流程...${NC}"

# 1. 从上游仓库同步最新代码
echo -e "${BLUE}从上游仓库同步最新代码...${NC}"
git fetch upstream
git merge upstream/main
if [ $? -ne 0 ]; then
    echo -e "${YELLOW}同步代码可能出现问题，请检查是否有冲突。${NC}"
    read -p "是否继续部署? [y/n]: " continue_deploy
    if [[ "$continue_deploy" != "y" && "$continue_deploy" != "Y" ]]; then
        echo -e "${RED}部署已取消。${NC}"
        exit 1
    fi
fi

# 2. 构建多架构Docker镜像
echo -e "${BLUE}构建多架构Docker镜像...${NC}"
./scripts/build-multiarch.sh
if [ $? -ne 0 ]; then
    echo -e "${RED}构建镜像失败，请检查错误信息。${NC}"
    exit 1
fi

# 3. 将升级脚本上传到服务器
echo -e "${BLUE}将升级脚本上传到服务器...${NC}"
scp scripts/docker-multiarch-upgrade.sh ${SERVER_USER}@${SERVER_IP}:${SERVER_PATH}/docker-upgrade.sh
if [ $? -ne 0 ]; then
    echo -e "${RED}上传脚本失败，请检查服务器连接。${NC}"
    exit 1
fi

# 4. 在服务器上执行升级
echo -e "${BLUE}在服务器上执行升级...${NC}"
ssh ${SERVER_USER}@${SERVER_IP} "cd ${SERVER_PATH} && chmod +x docker-upgrade.sh && sh docker-upgrade.sh"
if [ $? -ne 0 ]; then
    echo -e "${RED}服务器升级失败，请检查错误信息。${NC}"
    exit 1
fi

echo -e "${GREEN}部署流程已成功完成!${NC}"
echo -e "${GREEN}Midjourney Proxy 已经更新到最新版本。${NC}" 