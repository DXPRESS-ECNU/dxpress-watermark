﻿# DXPress Watermark Tool

该程序为大夏通讯社自动化图片水印程序。

## 更新说明

v0.1 8/30/2018 第一个发布版本，完成了基本功能。

v0.2 8/31/2018 实现了 `Linux` 及 `OSX` 上系统文件对话框的调用。

v0.3 12/24/2018 添加了对 GIF 动画及低分辨率图片的支持；统一了不同纵横比图片的水印的横向占比。

## 功能

该程序在执行过程中将完成以下事项：

1. 将图片长边压缩至2000px
2. 添加特定水印
3. 输出为 PNG/JPG/GIF

经测试，该程序亦可完成 GIF 动图的加水印工作。

## 使用方法

考虑到通讯社内较多同学使用 MacBook 等非 Windows 平台的计算机，该程序为跨平台应用，支持 `Windows / Linux / Mac OSX` ，使用过程中主要有以下几个步骤：

### 打开程序

`Windows` 用户运行程序根目录下 `Watermark.exe` 即可打开程序，`Linux` 及 `OSX` 用户运行根目录下 `Watermark` 程序。在 `Linux` 下时，若不能自行调起界面，请从终端中手动运行程序。

### 选择图片

将调用系统多选对话框，可按提示一次选择多个图片。

### 选择保存位置

将调用系统文件夹选择对话框，选择输出文件夹。

### 选择保存格式

按提示输入保存文件的格式，若不输入直接回车将默认为 PNG 格式。在微信推送中使用的图片推荐保存为 PNG 格式。

### 完成

确认开始后仅需等待程序运行完成即可。多图片处理时使用了多线程，可较的快完成批量处理。

## 附注

### 发布与更新

预编译包分为多个版本，按照末尾字符串区分：

1. `win-x64` 该版本为 `Windows` 使用的基于 `.Net Core 2.0` 的版本，自带依赖
2. `linux-x64` 该版本为 `Linux` 使用的基于 `.Net Core 2.0` 的版本，自带依赖
3. `osx-x64` 该版本为 `Mac OSX` 使用的基于 `.Net Core 2.0` 的版本，自带依赖
4. `netcore2.0` 该版本为跨平台通用的基于 `.Net Core 2.0` 的版本，未包含依赖，体积较小

该程序目前仅提供 x64 平台的预编译包，若有 x86 平台需求，请自行下载源代码编译。

程序源代码托管于 GitHub ，欢迎下载并提交 Pull Request。

预编译包可从 GitHub Release 中下载，同时也会上传至网盘之中。

Github 仓库地址： <https://github.com/DXPRESS-ECNU/dxpress-watermark>

### 问题反馈

若程序运行中出现问题，可在 GitHub 上提交 Issue，亦可直接联系开发者。
