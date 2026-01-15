import type { VbenFormSchema } from '#/adapter/form';

// 让 TypeScript 自动推断类型，不进行强制类型声明
const schemaModules = import.meta.glob('./schemas/*.ts', { 
  eager: false,
  import: 'useSchema'
});

export async function useModelingSchema(modelingName: string): Promise<VbenFormSchema[]> {
  const fileName = modelingName.charAt(0).toLowerCase() + modelingName.slice(1);
  const filePath = `./schemas/${fileName}.ts`;
  const moduleLoader = schemaModules[filePath];
  
  if (moduleLoader) {
    try {
      // 使用类型断言确保类型安全
      const importedModule = await moduleLoader() as unknown;
      
      if (typeof importedModule === 'function') {
        return (importedModule as () => VbenFormSchema[])();
      }
      
      console.error(`导入的模块不是函数: ${modelingName}`);
      return [];
    } catch (error) {
      console.error(`Failed to load schema for ${modelingName}:`, error);
      return [];
    }
  }
  
  return [];
}

export function getAvailableModelingTypes(): string[] {
  return Object.keys(schemaModules).map(filePath => {
    const fileName = filePath.replace('./schemas/', '').replace('.ts', '');
    return fileName.charAt(0).toUpperCase() + fileName.slice(1);
  });
}
