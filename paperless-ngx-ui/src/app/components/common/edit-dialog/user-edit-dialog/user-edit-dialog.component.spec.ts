import { HttpClientTestingModule } from '@angular/common/http/testing'
import { ComponentFixture, TestBed } from '@angular/core/testing'
import {
  FormsModule,
  ReactiveFormsModule,
  AbstractControl,
} from '@angular/forms'
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { NgSelectModule } from '@ng-select/ng-select'
import { of } from 'rxjs'
import { IfOwnerDirective } from 'src/app/directives/if-owner.directive'
import { IfPermissionsDirective } from 'src/app/directives/if-permissions.directive'
import { GroupService } from 'src/app/services/rest/group.service'
import { SettingsService } from 'src/app/services/settings.service'
import { PasswordComponent } from '../../input/password/password.component'
import { PermissionsFormComponent } from '../../input/permissions/permissions-form/permissions-form.component'
import { SelectComponent } from '../../input/select/select.component'
import { TextComponent } from '../../input/text/text.component'
import { PermissionsSelectComponent } from '../../permissions-select/permissions-select.component'
import { EditDialogMode } from '../edit-dialog.component'
import { UserEditDialogComponent } from './user-edit-dialog.component'

describe('UserEditDialogComponent', () => {
  let component: UserEditDialogComponent
  let settingsService: SettingsService
  let fixture: ComponentFixture<UserEditDialogComponent>

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [
        UserEditDialogComponent,
        IfPermissionsDirective,
        IfOwnerDirective,
        SelectComponent,
        TextComponent,
        PasswordComponent,
        PermissionsFormComponent,
        PermissionsSelectComponent,
      ],
      providers: [
        NgbActiveModal,
        {
          provide: GroupService,
          useValue: {
            listAll: () =>
              of({
                results: [
                  {
                    id: 1,
                    permissions: ['dummy_perms'],
                  },
                ],
              }),
          },
        },
        SettingsService,
      ],
      imports: [
        HttpClientTestingModule,
        FormsModule,
        ReactiveFormsModule,
        NgSelectModule,
        NgbModule,
      ],
    }).compileComponents()

    fixture = TestBed.createComponent(UserEditDialogComponent)
    settingsService = TestBed.inject(SettingsService)
    settingsService.currentUser = { id: 99, username: 'user99' }
    component = fixture.componentInstance

    fixture.detectChanges()
  })

  it('should support create and edit modes', () => {
    component.dialogMode = EditDialogMode.CREATE
    const createTitleSpy = jest.spyOn(component, 'getCreateTitle')
    const editTitleSpy = jest.spyOn(component, 'getEditTitle')
    fixture.detectChanges()
    expect(createTitleSpy).toHaveBeenCalled()
    expect(editTitleSpy).not.toHaveBeenCalled()
    component.dialogMode = EditDialogMode.EDIT
    fixture.detectChanges()
    expect(editTitleSpy).toHaveBeenCalled()
  })

  it('should disable user permissions select on toggle superuser', () => {
    const control: AbstractControl =
      component.objectForm.get('user_permissions')
    expect(control.disabled).toBeFalsy()
    component.objectForm.get('is_superuser').setValue(true)
    component.onToggleSuperUser()
    expect(control.disabled).toBeTruthy()
  })

  it('should update inherited permissions', () => {
    component.objectForm.get('groups').setValue(null)
    expect(component.inheritedPermissions).toEqual([])
    component.objectForm.get('groups').setValue([1])
    expect(component.inheritedPermissions).toEqual(['dummy_perms'])
    component.objectForm.get('groups').setValue([2])
    expect(component.inheritedPermissions).toEqual([])
  })

  it('should detect whether password was changed in form on save', () => {
    component.objectForm.get('password').setValue(null)
    component.save()
    expect(component.passwordIsSet).toBeFalsy()

    // unchanged pw
    component.objectForm.get('password').setValue('*******')
    component.save()
    expect(component.passwordIsSet).toBeFalsy()

    // unchanged pw
    component.objectForm.get('password').setValue('helloworld')
    component.save()
    expect(component.passwordIsSet).toBeTruthy()
  })
})
