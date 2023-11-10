import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing'
import { Subscription } from 'rxjs'
import { TestBed } from '@angular/core/testing'
import { environment } from 'src/environments/environment'
import { RemoteVersionService } from './remote-version.service'

let httpTestingController: HttpTestingController
let service: RemoteVersionService
let subscription: Subscription
const endpoint = 'remote_version'

describe('RemoteVersionService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RemoteVersionService],
      imports: [HttpClientTestingModule],
    })

    httpTestingController = TestBed.inject(HttpTestingController)
    service = TestBed.inject(RemoteVersionService)
  })

  afterEach(() => {
    subscription?.unsubscribe()
    httpTestingController.verify()
  })

  it('should call correct api endpoint on update check', () => {
    subscription = service.checkForUpdates().subscribe()
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}${endpoint}/`
    )
    expect(req.request.method).toEqual('GET')
  })
})
