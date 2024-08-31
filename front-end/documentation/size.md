# 尺寸与间距

## 尺寸


```
import themeStore from 'stepin、es/theme-editor/store'

const { setSize } = useThemeStore()
setSize({
  'side-width': '160px',               // 侧边宽度
  'side-collapsed-width': '64px',       // 侧边展开式宽度
  'header-height': '64px',              // 顶部高度
  'footer-height': '64px'              // 底部高度
});
```

## 外间距
以4个像素为基础单位，在此基础衍生出6个常用的外间距尺寸变量：`xxs`、`xs`、`sm`、`md`、`lg`、`xl`。

```
import themeStore from 'stepin/es/theme-editor/store'

const { setMargin } = useThemeStore()
/**
 * 设置外间距基础单位，生成对应的 margin 变量为：
 * xxs: 4px      (1 * base)
 * xs: 8px       (2 * base)
 * sm: 12px      (3 * base)
 * md: 16px      (4 * base)
 * lg: 24px      (6 * base)
 * xl: 32px      (8 * base)
 */
setMargin({base: '4px'});             

/**
 * 完全自定义外间距
 */
setMargin({
  xxs: '4px',
  xs: '8px',
  sm: '16px',
  md: '24px',
  lg: '32px',
  xl: '48px',
});
```  

我们为常用外间距尺寸提供了 css变量、tailwindcss 变量类以及less变量，详见 **css变量对照表**


## 内间距
和外间距一样，内间距以4个像素为基础单位，在此基础衍生出6个常用的内间距尺寸变量：`xxs`、`xs`、`sm`、`md`、`lg`、`xl`。


```
import themeStore from 'stepin/es/theme-editor/store'

const { setPadding } = useThemeStore()
/**
 * 设置外间距基础单位，生成对应的 margin 变量为：
 * xxs: 4px      (1 * base)
 * xs: 8px       (2 * base)
 * sm: 12px      (3 * base)
 * md: 16px      (4 * base)
 * lg: 24px      (6 * base)
 * xl: 32px      (8 * base)
 */
setPadding({base: '4px'});             

/**
 * 完全自定义外间距
 */
setPadding({
  xxs: '4px',
  xs: '8px',
  sm: '16px',
  md: '24px',
  lg: '32px',
  xl: '48px',
});
```  


我们也为常用内间距尺寸提供了 css变量、tailwindcss 变量类以及less变量，详见 **css变量对照表**
