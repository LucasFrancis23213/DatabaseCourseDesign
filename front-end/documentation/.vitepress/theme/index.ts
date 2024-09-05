// https://vitepress.dev/guide/custom-theme
import { h } from 'vue';
import Theme from 'vitepress/theme';
import MyLayout from './MyLayout.vue';
import './style.css';
import 'stepin/es/style';
import 'stepin/es/color-picker/style';
import 'stepin/es/code-sandbox/style';
import 'stepin/es/label-wrapper/style';
import { createPinia } from 'pinia';

export default {
  ...Theme,
  Layout: () => {
    return h(MyLayout, null, {
    });
  },
  enhanceApp({ app, router, siteData }) {
    const store = createPinia();
    app.use(store);
    // ...
  },
};
