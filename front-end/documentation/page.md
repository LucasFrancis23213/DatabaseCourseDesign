
# 目录结构

> 请先大致熟悉下项目目录结构  

```
├── docs                                      # 使用文档
├── public                                    # 公共静态资源
│   └── favicon                               # 网站图标
├── src
│   ├── assets                                # 本地静态资源
│   ├── components                            # 通用组件
│   ├── mock                                  # 本地 mock 数据
│   ├── pages                                 # 页面组件目录
│   ├── plugins                               # 插件
│   └── router                                # 路由配置
│       ├── dynamicRoutes.ts                  # 动态路由接口
│       ├── guards.ts                         # 路由守卫
│       ├── index.ts                          # 路由入口文件
│       └── routes.ts                         # 本地路由配置
│   ├── store                                 # vuex 状态管理配置
│   └── theme                                 # 主题配置
│       ├── antd                              # antd 样式覆盖
│       ├── style                             # 系统样式
│       ├── index.less                        # 系统样式入口文件
│       └── index.ts                          # 主题入口文件
│   ├── utils                                 # 工具类
│   ├── App.vue                               # 应用入口组件
│   └── main.ts                               # 应用入口js
├── .env                                      # 通用环境变量配置
├── .env.development                          # 开发环境变量配置
├── .prettierrc.json                          # prettierrc 配置文件
├── index.html                                # 应用入口 html
├── jsconfig.json                             # js开发配置
├── package.json                              # package.json
├── postcss.config.cjs                        # postcss 配置
├── README.md                                 # README.md
├── tailwind.config.cjs                       # tailwindcss 配置文件
├── tsconfig.json                             # typescript 配置文件
├── tsconfig.node.json                        # typescript 配置文件
└── vite.config.ts                            # vite 配置文件
```


