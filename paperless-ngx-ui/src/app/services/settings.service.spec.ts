import {
  HttpTestingController,
  HttpClientTestingModule,
} from '@angular/common/http/testing'
import { TestBed } from '@angular/core/testing'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { RouterTestingModule } from '@angular/router/testing'
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { CookieService } from 'ngx-cookie-service'
import { Subscription } from 'rxjs'
import { environment } from 'src/environments/environment'
import { AppModule } from '../app.module'
import {
  PaperlessUiSettings,
  SETTINGS_KEYS,
} from '../data/paperless-uisettings'
import { SettingsService } from './settings.service'
import { PaperlessSavedView } from '../data/paperless-saved-view'

describe('SettingsService', () => {
  let httpTestingController: HttpTestingController
  let settingsService: SettingsService
  let cookieService: CookieService
  let subscription: Subscription

  const ui_settings: PaperlessUiSettings = {
    user: {
      username: 'testuser',
      first_name: 'Test',
      last_name: 'User',
      id: 1,
      is_superuser: true,
    },
    settings: {
      language: '',
      bulk_edit: { confirmation_dialogs: true, apply_on_close: false },
      documentListSize: 50,
      dark_mode: { use_system: true, enabled: 'false', thumb_inverted: 'true' },
      theme: { color: '#9fbf2f' },
      document_details: { native_pdf_viewer: false },
      date_display: { date_locale: '', date_format: 'mediumDate' },
      notifications: {
        consumer_new_documents: true,
        consumer_success: true,
        consumer_failed: true,
        consumer_suppress_on_dashboard: true,
      },
      comments_enabled: true,
      slim_sidebar: false,
      update_checking: { enabled: false, backend_setting: 'default' },
      saved_views: { warn_on_unsaved_change: true },
      notes_enabled: true,
      tour_complete: false,
      permissions: {
        default_owner: null,
        default_view_users: [1],
        default_view_groups: [2],
        default_edit_users: [3],
        default_edit_groups: [4],
      },
    },
    permissions: [],
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [],
      providers: [SettingsService, CookieService],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule,
        NgbModule,
        FormsModule,
        ReactiveFormsModule,
        AppModule,
      ],
    })

    httpTestingController = TestBed.inject(HttpTestingController)
    cookieService = TestBed.inject(CookieService)
    settingsService = TestBed.inject(SettingsService)
  })

  afterEach(() => {
    subscription?.unsubscribe()
    httpTestingController.verify()
  })

  it('calls ui_settings api endpoint on initialize', () => {
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    expect(req.request.method).toEqual('GET')
  })

  it('calls ui_settings api endpoint with POST on store', () => {
    let req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(ui_settings)

    subscription = settingsService.storeSettings().subscribe()
    req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    expect(req.request.method).toEqual('POST')
    expect(req.request.body).toEqual({
      settings: ui_settings.settings,
    })
  })

  it('correctly loads settings of various types', () => {
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(ui_settings)

    expect(settingsService.displayName).toEqual('Test')
    expect(settingsService.getLanguage()).toEqual('')
    expect(settingsService.get(SETTINGS_KEYS.DARK_MODE_ENABLED)).toBeFalsy()
    expect(
      settingsService.get(SETTINGS_KEYS.NOTIFICATIONS_CONSUMER_NEW_DOCUMENT)
    ).toBeTruthy()
    expect(settingsService.get(SETTINGS_KEYS.DOCUMENT_LIST_SIZE)).toEqual(50)
    expect(settingsService.get(SETTINGS_KEYS.THEME_COLOR)).toEqual('#9fbf2f')
  })

  it('correctly allows updating settings of various types', () => {
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(ui_settings)

    settingsService.setLanguage('de-de')
    settingsService.set(SETTINGS_KEYS.DARK_MODE_ENABLED, true)
    settingsService.set(
      SETTINGS_KEYS.NOTIFICATIONS_CONSUMER_NEW_DOCUMENT,
      false
    )
    settingsService.set(SETTINGS_KEYS.DOCUMENT_LIST_SIZE, 25)
    settingsService.set(SETTINGS_KEYS.THEME_COLOR, '#000000')

    expect(settingsService.getLanguage()).toEqual('de-de')
    expect(settingsService.get(SETTINGS_KEYS.DARK_MODE_ENABLED)).toBeTruthy()
    expect(
      settingsService.get(SETTINGS_KEYS.NOTIFICATIONS_CONSUMER_NEW_DOCUMENT)
    ).toBeFalsy()
    expect(settingsService.get(SETTINGS_KEYS.DOCUMENT_LIST_SIZE)).toEqual(25)
    expect(settingsService.get(SETTINGS_KEYS.THEME_COLOR)).toEqual('#000000')
  })

  it('sets django cookie for languages', () => {
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
    const cookieSetSpy = jest.spyOn(cookieService, 'set')
    settingsService.initializeSettings().subscribe(() => {})
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    ui_settings.settings['language'] = 'foobar'
    req.flush(ui_settings)
    expect(cookieSetSpy).toHaveBeenCalledWith('django_language', 'foobar')
    const cookieDeleteSpy = jest.spyOn(cookieService, 'delete')
    settingsService.setLanguage('')
    expect(cookieDeleteSpy).toHaveBeenCalled()
  })

  it('should support null values for settings if set, undefined if not', () => {
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
    expect(settingsService.get('foo')).toEqual(undefined)
    expect(settingsService.get(SETTINGS_KEYS.DEFAULT_PERMS_OWNER)).toEqual(null)
  })

  it('should support array values', () => {
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
    expect(settingsService.get(SETTINGS_KEYS.DEFAULT_PERMS_VIEW_USERS)).toEqual(
      [1]
    )
  })

  it('should support default permissions values', () => {
    delete ui_settings.settings['permissions']
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
    expect(settingsService.get(SETTINGS_KEYS.DEFAULT_PERMS_OWNER)).toEqual(1)
    expect(settingsService.get(SETTINGS_KEYS.DEFAULT_PERMS_VIEW_USERS)).toEqual(
      []
    )
  })

  it('updates appearance settings', () => {
    const req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(ui_settings)

    expect(
      document.documentElement.style.getPropertyValue(
        '--pngx-primary-lightness'
      )
    ).toEqual('')

    const addClassSpy = jest.spyOn(settingsService.renderer, 'addClass')
    const setAttributeSpy = jest.spyOn(settingsService.renderer, 'setAttribute')

    settingsService.updateAppearanceSettings(true, true, '#fff000')
    expect(addClassSpy).toHaveBeenCalledWith(document.body, 'primary-light')
    expect(setAttributeSpy).toHaveBeenCalledWith(
      document.documentElement,
      'data-bs-theme',
      'auto'
    )
    expect(
      document.documentElement.style.getPropertyValue(
        '--pngx-primary-lightness'
      )
    ).toEqual('50%')

    settingsService.updateAppearanceSettings(false, false, '#000000')
    expect(addClassSpy).toHaveBeenCalledWith(document.body, 'primary-light')
    expect(setAttributeSpy).toHaveBeenCalledWith(
      document.documentElement,
      'data-bs-theme',
      'light'
    )

    expect(
      document.documentElement.style.getPropertyValue(
        '--pngx-primary-lightness'
      )
    ).toEqual('0%')

    settingsService.updateAppearanceSettings(false, true, '#ffffff')
    expect(addClassSpy).toHaveBeenCalledWith(document.body, 'primary-dark')
    expect(setAttributeSpy).toHaveBeenCalledWith(
      document.documentElement,
      'data-bs-theme',
      'dark'
    )
    expect(
      document.documentElement.style.getPropertyValue(
        '--pngx-primary-lightness'
      )
    ).toEqual('100%')
  })

  it('migrates settings automatically', () => {
    const oldSettings = Object.assign({}, ui_settings)
    delete oldSettings.settings['documentListSize']
    window.localStorage.setItem(SETTINGS_KEYS.DOCUMENT_LIST_SIZE, '50')
    let req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(oldSettings)

    req = httpTestingController.match(
      `${environment.apiBaseUrl}ui_settings/`
    )[0]
    expect(req.request.method).toEqual('POST')
  })

  it('updates settings on complete tour', () => {
    let req = httpTestingController.expectOne(
      `${environment.apiBaseUrl}ui_settings/`
    )
    req.flush(ui_settings)

    settingsService.completeTour()

    req = httpTestingController.match(
      `${environment.apiBaseUrl}ui_settings/`
    )[0]
    expect(req.request.method).toEqual('POST')
  })

  it('should update saved view sorting', () => {
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
    const setSpy = jest.spyOn(settingsService, 'set')
    settingsService.updateDashboardViewsSort([
      { id: 1 } as PaperlessSavedView,
      { id: 4 } as PaperlessSavedView,
    ])
    expect(setSpy).toHaveBeenCalledWith(
      SETTINGS_KEYS.DASHBOARD_VIEWS_SORT_ORDER,
      [1, 4]
    )
    httpTestingController
      .expectOne(`${environment.apiBaseUrl}ui_settings/`)
      .flush(ui_settings)
  })
})
