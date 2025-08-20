import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersDashBoard } from './users-dash-board';

describe('UsersDashBoard', () => {
  let component: UsersDashBoard;
  let fixture: ComponentFixture<UsersDashBoard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsersDashBoard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsersDashBoard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
