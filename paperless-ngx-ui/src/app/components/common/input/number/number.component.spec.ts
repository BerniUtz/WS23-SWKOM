import { ComponentFixture, TestBed } from '@angular/core/testing'
import {
  FormsModule,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms'
import { NumberComponent } from './number.component'
import { DocumentService } from 'src/app/services/rest/document.service'
import { HttpClientTestingModule } from '@angular/common/http/testing'
import { of } from 'rxjs'

describe('NumberComponent', () => {
  let component: NumberComponent
  let fixture: ComponentFixture<NumberComponent>
  let input: HTMLInputElement
  let documentService: DocumentService

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [NumberComponent],
      providers: [DocumentService],
      imports: [FormsModule, ReactiveFormsModule, HttpClientTestingModule],
    }).compileComponents()

    fixture = TestBed.createComponent(NumberComponent)
    fixture.debugElement.injector.get(NG_VALUE_ACCESSOR)
    component = fixture.componentInstance
    documentService = TestBed.inject(DocumentService)
    fixture.detectChanges()
    input = component.inputField.nativeElement
  })

  it('should support +1 ASN', () => {
    const nextAsnSpy = jest.spyOn(documentService, 'getNextAsn')
    nextAsnSpy.mockReturnValueOnce(of(1001)).mockReturnValueOnce(of(1))
    expect(component.value).toBeUndefined()
    component.nextAsn()
    expect(component.value).toEqual(1001)

    // this time results are empty
    component.value = undefined
    component.nextAsn()
    expect(component.value).toEqual(1)

    component.value = 1002
    component.nextAsn()
    expect(component.value).toEqual(1002)
  })
})
