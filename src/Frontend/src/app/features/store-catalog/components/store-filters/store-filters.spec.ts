import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoreFilters } from './store-filters';

describe('StoreFilters', () => {
  let component: StoreFilters;
  let fixture: ComponentFixture<StoreFilters>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoreFilters]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoreFilters);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
