import type { VbenFormSchema } from '#/adapter/form';
import { useFooterSchema } from './footer';
import { useHeaderSchema } from './header';

export function useFactorySchema(): VbenFormSchema[] {
  let schema: VbenFormSchema[] = []

  return [...useHeaderSchema(), ...schema, ...useFooterSchema()];
}
