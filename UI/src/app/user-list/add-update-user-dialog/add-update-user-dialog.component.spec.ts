import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddUpdateUserDialogComponent } from './add-update-user-dialog.component';

describe('AddUpdateUserDialogComponent', () => {
  let component: AddUpdateUserDialogComponent;
  let fixture: ComponentFixture<AddUpdateUserDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddUpdateUserDialogComponent]
    });
    fixture = TestBed.createComponent(AddUpdateUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
