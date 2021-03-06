import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Store } from '@ngrx/store';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { RootState } from 'src/app/root/reducers';
import { fromAdvertise, fromAdvertiseAPI } from '../actions';
import { AdvertiseService } from '../services/advertise.service';

@Injectable({
  providedIn: 'root',
})
export class AdvertiseExistGuard implements CanActivate {
  // loadItemFromEntity$: Observable<'' | IResponseAdvertise | null | undefined>;

  constructor(
    private store: Store<RootState>,
    private advertiseService: AdvertiseService,
    private router: Router,
    private notification: NzNotificationService
  ) {
    // this.loadItemFromEntity$ = this.store.pipe(
    //   select(getSelectedAdvertiseIdEntity)
    // );
  }
  // hasAdvertiseInStore(): Observable<boolean> {
  //   return this.loadItemFromEntity$.pipe(
  //     map((x) => !!x),
  //     take(1)
  //   );
  // }

  hasAdvertiseInAPI(id: string): Observable<boolean> {
    return this.advertiseService.getAdvetise(id).pipe(
      map((advertise) =>
        fromAdvertiseAPI.loadAdvertiseFromAPISuccess({ advertise })
      ),
      tap((action) => this.store.dispatch(action)),
      tap((action) =>
        this.store.dispatch(
          fromAdvertise.selectAdvertise({
            uniqueId: action.advertise.userAdvertise.advertise.uniqueId,
          })
        )
      ),
      map((advertise) => !!advertise),

      catchError((error: HttpErrorResponse) => {
        if (error.status === 0) {
          this.notification.error(
            'Server is down',
            'Unable to connect to server',
            {
              nzPlacement: 'bottomLeft',
            }
          );
        }
        if (error.status === 401) {
          this.router.navigate(['/auth/login'], {
            queryParams: { redirectView: id },
          });
        }

        return of(false);
      })
    );
  }

  // isAdvertiseExist(id: string): Observable<boolean> {
  //   return this.hasAdvertiseInStore().pipe(
  //     switchMap((advertiseInStore) => {
  //       if (advertiseInStore) {
  //         return of(advertiseInStore);
  //       } else {
  //         return this.hasAdvertiseInAPI(id);
  //       }
  //     })
  //   );
  // }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree> {
    return this.hasAdvertiseInAPI(route.params.id);
  }
}
