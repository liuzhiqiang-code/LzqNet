import type { VbenFormSchema } from '#/adapter/form';
import { useFooterSchema } from './footer';
import { useHeaderSchema } from './header';

export function useSchema(): VbenFormSchema[] {
  let schema: VbenFormSchema[] = []

  return [...useHeaderSchema(), ...schema, ...useFooterSchema()];
}
