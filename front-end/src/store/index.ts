import { createPinia } from 'pinia';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate';
export { storeToRefs } from 'pinia';
export * from './account';
export * from './menu';
export * from './setting';
export * from './loading';

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

export default pinia;
