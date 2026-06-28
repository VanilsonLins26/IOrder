import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreInfoHeader } from './store-info-header';

describe('StoreInfoHeader', () => {
  let component: StoreInfoHeader;
  let fixture: ComponentFixture<StoreInfoHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoreInfoHeader]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoreInfoHeader);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
