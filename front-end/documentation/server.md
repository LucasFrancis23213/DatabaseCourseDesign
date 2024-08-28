# 接口功能
> 本项目使用 `axios` 来处理 http 请求  

## 创建请求

详情请参考 `src/utils/axiosHttp.ts` 的代码  

下面的代码创建了一个 http 实例

```
import createHttp from '@/utils/axiosHttp';

const http = createHttp({
  timeout: 10000,
  baseURL: 'api',
  withCredentials: true,
  xsrfCookieName: 'Authorization',
  xsrfHeaderName: 'Authorization',
});
```  

它的创建和 axios 配置一样，当然也可以创建多个 http 实例，用于处理不同 host 的接口服务。
## token
给 http 实例添加了几个 token 管理接口：
### `http.setAuthorization`
用于设置该http实例的token，设置后该实例的请求会自动带上 token 请求头用于身份验证。
```
/**
 * 设置token
 * @param value token值
 * @param expires 过期时间
 * - 类型为 number 时，单位为毫秒，表示 {expires} 毫秒后 token 过期
 * - 类型为 Date 时，表示在 {expires} 这个时间点 token 过期
 * @param name token 名称，默认为当前 http 实例的 xsrfCookieName 属性值
 */
setAuthorization(value: string, expires: number | Date, name?: string): void;
```

###  `http.removeAuthorization`
用于删除http实例的token，一般在登出等注销操作时调用。
```ts
/**
 * 移出token
 * @param name token 名称， 默认为当前 http 实例的 xsrfCookieName 属性值
 */
removeAuthorization(name?: string): void;
```

### `http.checkAuthorization`
校验 token 是否有效，一般用于接口请求前的校验或页面跳转前的校验

```
/**
 * 校验 token 是否有效
 * @param name 需要校验的 token 名称，默认为当前 http 实例的 xsrfCookieName 属性值
 */
checkAuthorization(name?: string): boolean;
```
## 拦截器  

拦截器用法与 axios 拦截器完全一致，下面是几个拦截器示例代码：

```
...
// 解析响应结果
http.interceptors.response.use(
  (rep: AxiosResponse<String>) => {
    const { data } = rep;
    if (isResponse(data)) {
      return data.code === 0 ? data : Promise.reject(data);
    }
    return Promise.reject({ message: rep.statusText, code: rep.status, data });
  },
  (error) => {
    if (error.response && isAxiosResponse(error.response)) {
      return Promise.reject({
        message: error.response.statusText,
        code: error.response.status,
        data: error.response.data,
      });
    }
    return Promise.reject(error);
  }
);

// progress 进度条 -- 开启
http.interceptors.request.use((req: AxiosRequestConfig) => {
  if (!NProgress.isStarted()) {
    NProgress.start();
  }
  return req;
});

// progress 进度条 -- 关闭
http.interceptors.response.use(
  (rep) => {
    if (NProgress.isStarted()) {
      NProgress.done();
    }
    return rep;
  },
  (error) => {
    if (NProgress.isStarted()) {
      NProgress.done();
    }
    return error;
  }
);
...
```

