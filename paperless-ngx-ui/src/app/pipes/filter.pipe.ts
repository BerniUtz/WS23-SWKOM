import { Pipe, PipeTransform } from '@angular/core'
import { MatchingModel } from '../data/matching-model'

@Pipe({
  name: 'filter',
})
export class FilterPipe implements PipeTransform {
  transform(items: MatchingModel[], searchText: string): MatchingModel[] {
    if (!items) return []
    if (!searchText) return items

    return items.filter((item) => {
      return Object.keys(item).some((key) => {
        return String(item[key])
          .toLowerCase()
          .includes(searchText.toLowerCase())
      })
    })
  }
}
